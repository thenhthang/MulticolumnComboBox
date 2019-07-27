// Copyright © Serge Weinstock 2014.
//
// This library is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ComboBox = System.Windows.Forms.ComboBox;
using System.Linq;
namespace UMCComboBox
{
    /// <summary>
    /// This is a combobox with a suggestion list à la "Sublime Text"
    /// 
    /// Searches are made against the pattern in the combo textbox by matching
    /// all the characters in the pattern in the right order but not consecutively
    /// </summary>
    public class EasyUMCComboBox : ComboBox
    {
        #region fields and properties
        /// <summary>our custom drowp down control</summary>
        private readonly DropdownControl m_dropDown;
        /// <summary>the suggestion list inside the drop down control</summary>
        private readonly ListBox m_suggestionList;
        /// <summary>the bold font used for drawing strings in the listbox</summary>
        private Font m_boldFont;
        /// <summary>Allows to know if the last text change is triggered by the keyboard</summary>
        private bool m_fromKeyboard;
        /// <summary>How do we match user input to strings?</summary>
        private StringMatchingMethod m_matchingMethod;
        private bool m_suggestAppend = true;
        private string m_firstText = "";
        // Row and column sizes.
        private float RowWidth;


        private const float RowMargin = 10;
        private const float ColumnMargin = 10;
        private float[] colWidths = { 70, 70, 70 };
        private string[] arrColumnsName;
        private string[] arrFormatColumns;
        // The column alignments.
        private StringAlignment[] VertAlignments =
        {
            StringAlignment.Center,
            StringAlignment.Center,
            StringAlignment.Center,
            StringAlignment.Center,
            StringAlignment.Center,
        };
        private StringAlignment[] HorzAlignments =
        {
            StringAlignment.Near,
            StringAlignment.Near,
            StringAlignment.Far,
            StringAlignment.Far,
            StringAlignment.Far,
        };
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public EasyUMCComboBox()
        {
            m_matchingMethod = StringMatchingMethod.UseWildcards;
            // we're overriding these
            DropDownStyle = ComboBoxStyle.DropDown;
            AutoCompleteMode = AutoCompleteMode.None;
            // let's build our suggestion list
            m_suggestionList = new ListBox
            {
                DisplayMember = "Text",
                TabStop = false, 
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawVariable,
                IntegralHeight = true,
                Sorted = Sorted,
                
            };
            m_suggestionList.MeasureItem += new MeasureItemEventHandler(m_suggestionList_MeasureItem);
            m_suggestionList.Click += onSuggestionListClick;
            m_suggestionList.DrawItem += onSuggestionListDrawItem;
            FontChanged += onFontChanged;
            m_suggestionList.MouseMove += onSuggestionListMouseMove;
            m_dropDown = new DropdownControl(m_suggestionList);
            onFontChanged(null, null);
            
        }
        protected override void InitLayout()
        {
            string[] tokens = this.colWidthsString.Split(';');
            this.colWidths = Array.ConvertAll<string, float>(tokens, float.Parse);
            this.arrColumnsName = DisplayColumns.Split(';');
            this.arrFormatColumns = formatColumns.Split(';');
            this.RowWidth = this.colWidths.Sum();
            base.InitLayout();
        }
        // Return the desired size of an item.
        void m_suggestionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            //// Measure the data if we haven't already done so.
            //if (ColWidths == null)
            //{
            //    // Get the row and column sizes.
            //    GetRowColumnSizes(e.Graphics, m_suggestionList.Font, Values,
            //        out RowHeight, out ColWidths);

            //    // Add margins.
            //    for (int i = 0; i < ColWidths.Length; i++)
            //        ColWidths[i] += ColumnMargin;
            //    RowHeight += RowMargin;

            //    // Get the total row width.
            //    RowWidth = ColWidths.Sum();
            //}

            //// Set the desired size.
           // RowHeight += RowMargin;
            e.ItemHeight = (int)this.rowHeight;
            e.ItemWidth = (int)RowWidth;
        }
        // Return the items' sizes.
        private void GetRowColumnSizes(Graphics gr, Font font, string[][] values,
            out float max_height, out float[] col_widths)
        {
            // Make room for the column sizes.
            int num_cols = values[0].Length;
            col_widths = new float[num_cols];

            // Examine each row.
            max_height = 0;
            foreach (string[] row in values)
            {
                // Measure the row's columns.
                for (int col_num = 0; col_num < num_cols; col_num++)
                {
                    SizeF col_size = gr.MeasureString(row[col_num], font);
                    if (col_widths[col_num] < col_size.Width)
                        col_widths[col_num] = col_size.Width;
                    if (max_height < col_size.Height)
                        max_height = col_size.Height;
                }
            }
        }
        /// <summary>
        /// <see cref="ComboBox.Dispose(bool)"/>
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_boldFont != null)
                {
                    m_boldFont.Dispose();
                }
                m_dropDown.Dispose();
            }
            base.Dispose(disposing);
        }

        #region size and position of suggest box
        /// <summary>
        /// <see cref="ComboBox.OnLocationChanged(EventArgs)"/>
        /// </summary>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            hideDropDown();
        }

        /// <summary>
        /// <see cref="ComboBox.OnSizeChanged(EventArgs)"/>
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            m_dropDown.Width = Width;
        }
        #endregion

        #region visibility of suggest box
        /// <summary>
        /// Shows the drop down.
        /// </summary>
        public void showDropDown()
        {
            this.m_suggestAppend = false;
            if (DesignMode)
            {
                return;
            }
            // Hide the "standard" drop down if any
            if (base.DroppedDown)
            {
                BeginUpdate();
                // setting DroppedDown to false may select an item
                // so we save the editbox state
                string oText = Text;
                int selStart = SelectionStart;
                int selLen =SelectionLength;

                // close the "standard" dropdown
                base.DroppedDown = false;
                
                // and restore the contents of the editbox

                Text = oText;
                Select(selStart, selLen);
                
                EndUpdate();
            }
            // pop it up and resize it
            //int h = Math.Min(MaxDropDownItems, m_suggestionList.Items.Count) * m_suggestionList.ItemHeight;
            int h = Math.Min(MaxDropDownItems, m_suggestionList.Items.Count) * (int)this.rowHeight;
            m_dropDown.Show(this, new Size(DropDownWidth, h));
            this.m_suggestAppend = true;
        }

        //public void showDropDown()
        //{
        //    this.m_suggestAppend = false;
        //    if (DesignMode)
        //    {
        //        return;
        //    }
        //    // Hide the "standard" drop down if any
        //    if (base.DroppedDown)
        //    {
        //        BeginUpdate();
        //        // setting DroppedDown to false may select an item
        //        // so we save the editbox state
        //        string oText = Text;
        //        int selStart = SelectionStart;
        //        int selLen = SelectionLength;

        //        // close the "standard" dropdown
        //        base.DroppedDown = false;

        //        // and restore the contents of the editbox

        //        Text = oText;
        //        Select(selStart, selLen);

        //        EndUpdate();
        //    }
        //    // pop it up and resize it
        //    //int h = Math.Min(MaxDropDownItems, m_suggestionList.Items.Count) * m_suggestionList.ItemHeight;
        //    int h = Math.Min(MaxDropDownItems, m_suggestionList.Items.Count) * (int)this.rowHeight;
        //    m_dropDown.Show(this, new Size(DropDownWidth, h));
        //}
        /// <summary>
        /// Hides the drop down.
        /// </summary>
        public void hideDropDown()
        {
            if (m_dropDown.Visible)
            {
                m_dropDown.Close();
            }
        }

        /// <summary>
        /// <see cref="ComboBox.OnLostFocus(EventArgs)"/>
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!m_dropDown.Focused && !m_suggestionList.Focused)
            {
                hideDropDown();
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// <see cref="ComboBox.OnDropDown(EventArgs)"/>
        /// </summary>
        protected override void OnDropDown(EventArgs e)
        {
            hideDropDown();
            base.OnDropDown(e);
        }
        #endregion

        #region keystroke and mouse events
        /// <summary>
        /// Called when the user clicks on an item in the list
        /// </summary>
        private void onSuggestionListClick(object sender, EventArgs e)
        {
            m_fromKeyboard = false;
            StringMatch sel = (StringMatch)m_suggestionList.SelectedItem;
            Text = sel.Text;
            Select(0, Text.Length);
            Focus();
        }

        /// <summary>
        /// Process command keys
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Tab) && (m_dropDown.Visible))
            {
                if (m_suggestionList.SelectedIndex < 0 && Text.Length > 0)//Them
                {
                    m_suggestionList.SelectedIndex = 0;
                }
                // we change the selection but will also allow the navigation to the next control
                if (m_suggestionList.Text.Length != 0)
                {
                    Text = m_suggestionList.Text;
                }
                Select(0, Text.Length);
                hideDropDown();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// if the dropdown is visible some keystrokes
        /// should behave in a custom way
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            m_fromKeyboard = true;

            if (!m_dropDown.Visible)
            {
                base.OnKeyDown(e);
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (m_suggestionList.SelectedIndex < 0)
                    {
                        m_suggestionList.SelectedIndex = 0;
                    }
                    else if (m_suggestionList.SelectedIndex < m_suggestionList.Items.Count - 1)
                    {
                        m_suggestionList.SelectedIndex++;
                    }
                    break;
                case Keys.Up:
                    if (m_suggestionList.SelectedIndex > 0)
                    {
                        m_suggestionList.SelectedIndex--;
                    }
                    else if (m_suggestionList.SelectedIndex < 0)
                    {
                        m_suggestionList.SelectedIndex = m_suggestionList.Items.Count - 1;
                    }
                    break;
                case Keys.Enter:
                    if (m_suggestionList.Text.Length != 0)
                    {
                        Text = m_suggestionList.Text;
                    }
                    Select(0, Text.Length);
                    hideDropDown();
                    break;
                case Keys.Escape:
                    hideDropDown();
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// We need to know if the last text changed event was due to one of the dropdowns 
        /// or to the keyboard
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDownClosed(EventArgs e)
        {
            m_fromKeyboard = false;
            base.OnDropDownClosed(e);
        }
        private bool searchAllColums = false;
        [Description("Tim kien tat ca ca cot trong datasource")]
        public bool SearchAllColumns {
            get {   return this.searchAllColums;  }
            set {   this.searchAllColums = value;}
        }
        private string searchColumnName = "";
        [Description("Ten cot can tim")]
        public string SearchColumnName
        {
            get { return this.searchColumnName; }
            set { this.searchColumnName = value; }
        }

        private string colWidthsString = "";
        [Description("Chieu rong cac cot. ex: 400;200;100;...")]
        public string ColWidthsString
        {
            get { return this.colWidthsString; }
            set {
                this.colWidthsString = value;
            }
        }
        private string displayColumns = "";
        [Description("Ten cot hien thi. columnName1;columnName2;...")]
        public string DisplayColumns
        {
            get { return this.displayColumns; }
            set
            {
                this.displayColumns = value;
            }
        }
        private string formatColumns = "";
        [Description("Kieu du lieu tung cot. string;number;...")]
        public string FormatColumns
        {
            get { return this.formatColumns; }
            set
            {
                this.formatColumns = value;
            }
        }
        private float rowHeight = 20;
        [Description("Chieu cao dong")]
        public float RowHeight
        {
            get { return this.rowHeight; }
            set
            {
                this.rowHeight = value;
            }
        }
        private float minLengthTextSearch = 3;
        [Description("So ky tu toi thieu de bat dau tim kiem")]
        public float MinLengthTextSearch
        {
            get { return this.minLengthTextSearch; }
            set
            {
                this.minLengthTextSearch = value;
            }
        }
        /// <summary>
        /// this were we can make suggestions
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (!m_fromKeyboard || !Focused)
            {
                return;
            }
            if (Text.Length < this.minLengthTextSearch)
                return;
            m_suggestionList.BeginUpdate();
            m_suggestionList.Items.Clear();
            StringMatcher matcher = new StringMatcher(MatchingMethod, Text);
            //Chỉ Tìm kiếm ở DisplayMember
            if (this.searchAllColums == false)
            {
                foreach (System.Data.DataRowView item in Items)
                {
                    StringMatch sm = matcher.Match(
                        searchColumnName==""?
                        GetItemText(item):
                        item[searchColumnName].ToString());
                    if (sm != null)
                    {
                        sm.Text = GetItemText(item);
                        m_firstText = (m_firstText == "" ? sm.Text : m_firstText);
                        sm.Segments.Clear();
                        for (int i = 0; i<arrFormatColumns.Length; i++)
                        {
                            sm.Segments.Add(
                                arrFormatColumns[i] == "string"?
                                item[arrColumnsName[i]].ToString():
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-vn"), "{0:#,0.##}", item[arrColumnsName[i]]) );
                            //var ci = new System.Globalization.CultureInfo("vi-vn");
                            //string.Format(ci, "{0:#,0.##}", item[arrColumnsName[i]]);
                        }
                        sm.StartsOnMatch = false;
                        m_suggestionList.Items.Add(sm);
                    }
                }
                //m_sringMatchSort = m_sringMatchSort.OrderBy(p => p.indexMatch).ToList();
                //foreach (StringMatchSort m in m_sringMatchSort)
                //{
                //    m_suggestionList.Items.Add(m.stringMatch);
                //}
            }
            //Tìm kiếm trên tất cả các cột của DataSource
            else
            {
                foreach (System.Data.DataRowView item in Items)
                {
                    string data = "";
                    foreach (string name in arrColumnsName)
                    {
                        data = data + item[name].ToString() + " ";
                    }
                    StringMatch sm2 = matcher.Match(data);
                    if (sm2 != null)
                    {
                        sm2.Text = GetItemText(item);

                        sm2.Segments.Clear();
                        for (int i = 0; i < arrFormatColumns.Length; i++)
                        {
                            sm2.Segments.Add(arrFormatColumns[i] == "string" ?
                                item[arrColumnsName[i]].ToString() :
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-vn"), "{0:#,0.##}", item[arrColumnsName[i]]));
                        }
                        sm2.StartsOnMatch = false;
                        m_suggestionList.Items.Add(sm2);
                    }
                }
            }
            m_suggestionList.EndUpdate();
            bool visible = m_suggestionList.Items.Count != 0;
            if (m_suggestionList.Items.Count == 1 && ((StringMatch)m_suggestionList.Items[0]).Text.Length == Text.Trim().Length)
            {
                StringMatch sel = (StringMatch)m_suggestionList.Items[0];
                Text = sel.Text;
                Select(0, Text.Length);
                visible = false;
            }
            if (visible)
            {
                showDropDown();
            }
            else
            {
                hideDropDown();                
            }
            m_fromKeyboard = false;
        }

        /// <summary>
        /// We highlight the selection under the mouse in the suggestion listbox
        /// </summary>
        private void onSuggestionListMouseMove(object sender, MouseEventArgs e)
        {
            int idx = m_suggestionList.IndexFromPoint(e.Location);
            if ((idx >= 0) && (idx != m_suggestionList.SelectedIndex))
            {
                m_suggestionList.SelectedIndex = idx;
            }
        }
        #endregion

        #region owner drawn
        /// <summary>
        /// We keep track of system settings changes for the font
        /// </summary>
        private void onFontChanged(object sender, EventArgs e)
        {
            if (m_boldFont != null)
            {
                m_boldFont.Dispose();
            }
            m_suggestionList.Font = Font;
            m_boldFont = new Font(Font, FontStyle.Bold);
            m_suggestionList.ItemHeight = m_boldFont.Height + 2;
        }

        /// <summary>
        /// Draw a segment of a string and updates the bound rectangle for being used for the next segment drawing
        /// </summary>
        private static void DrawString(Graphics g, Color color, ref Rectangle rect, string text, Font font)
        {
            Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            Size sz = TextRenderer.MeasureText(g, text, font, proposedSize, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, text, font, rect, color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            rect.X += sz.Width;
            rect.Width -= sz.Width;
        }

        /// <summary>
        /// Draw an item in the suggestion listbox
        /// </summary>
        private void onSuggestionListDrawItem(object sender, DrawItemEventArgs e)
        {
            //StringMatch sm = (StringMatch)m_suggestionList.Items[e.Index];
            //e.DrawBackground();
            //bool isBold = sm.StartsOnMatch;
            //Rectangle rBounds = e.Bounds;
            //foreach (string s in sm.Segments)
            //{
            //    Font f = isBold ? m_boldFont : Font;
            //    DrawString(e.Graphics, e.ForeColor, ref rBounds, s, f);
            //    isBold = !isBold;
            //}
            //e.DrawFocusRectangle();
            Pen pen = new Pen(Color.FromArgb(0xEC, 0xEB, 0xEB), 1);
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Outset; //<-- this
            string[] values = ((StringMatch)m_suggestionList.Items[e.Index]).Segments.ToArray();
            e.DrawBackground();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                DrawRow(e.Graphics, m_suggestionList.Font, SystemBrushes.HighlightText,
                    pen, e.Bounds.X, e.Bounds.Y, this.rowHeight, colWidths,
                    VertAlignments, HorzAlignments, values, true);
            }
            else
            {
                DrawRow(e.Graphics, m_suggestionList.Font, Brushes.Black, pen,
                    e.Bounds.X, e.Bounds.Y, this.rowHeight, colWidths,
                    VertAlignments, HorzAlignments, values, true);
            }
        }
        // Draw the items in columns.
        private void DrawRow(Graphics gr, Font font, Brush brush, Pen box_pen,
            float x0, float y0, float row_height, float[] col_widths,
            StringAlignment[] vert_alignments, StringAlignment[] horz_alignments,
            string[] values, bool draw_box)
        {
            // Create a rectangle in which to draw.
            RectangleF rect = new RectangleF();
            rect.Height = row_height;
          

            using (StringFormat sf = new StringFormat())
            {
                float x = x0;
                for (int col_num = 0; col_num < values.Length; col_num++)
                {
                    // Prepare the StringFormat and drawing rectangle.
                    sf.Alignment = horz_alignments[col_num];
                    sf.LineAlignment = vert_alignments[col_num];
                    rect.X = x;
                    rect.Y = y0;
                    rect.Width = col_widths[col_num];
                    
                    // Draw.
                    gr.DrawString(values[col_num], font, brush, rect, sf);

                    // Draw the box if desired.
                    if (draw_box) gr.DrawRectangle(box_pen,
                        rect.X, rect.Y, rect.Width, rect.Height);

                    // Move to the next column.
                    x += col_widths[col_num];
                }
            }
        }
        #endregion
        
        #region misc
        [Category("Behavior"), DefaultValue(false), Description("Specifies whether items in the list portion of the combobo are sorted.")]
        public new bool Sorted
        {
            get { return base.Sorted; }
            set
            {
                m_suggestionList.Sorted = value;
                base.Sorted = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool DroppedDown
        {
            get { return base.DroppedDown || m_dropDown.Visible; }
            set 
            { 
                m_dropDown.Visible = false;
                base.DroppedDown = value;
            }
        }
        #endregion

        #region New properties
        [
            DefaultValue(StringMatchingMethod.NoWildcards),
            Description("How strings are matched against the user input"),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always),
            Category("Behavior")
        ]
        public StringMatchingMethod MatchingMethod
        {
            get { return m_matchingMethod;  }
            set
            {
                if (m_matchingMethod != value)
                {
                    m_matchingMethod = value;
                    if (m_dropDown.Visible)
                    {
                        // recalculate the matches
                        showDropDown();
                    }
                }
            }
        }
        #endregion
        
        #region Hidden inherited properties
        /// <summary>This property is not relevant for this class.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoCompleteSource AutoCompleteSource
        {
            get { return base.AutoCompleteSource; }
            set { base.AutoCompleteSource = value; }
        }
        /// <summary>This property is not relevant for this class.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoCompleteStringCollection AutoCompleteCustomSource 
        {
            get { return base.AutoCompleteCustomSource; }
            set { base.AutoCompleteCustomSource = value; }
        }
        /// <summary>This property is not relevant for this class.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoCompleteMode AutoCompleteMode
        {
            get { return base.AutoCompleteMode; }
            set { base.AutoCompleteMode = value; }
        }
        /// <summary>This property is not relevant for this class.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new ComboBoxStyle DropDownStyle
        {
            get { return base.DropDownStyle; }
            set { base.DropDownStyle = value; }
        }
        #endregion
    }
}