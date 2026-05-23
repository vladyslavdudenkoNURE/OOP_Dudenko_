using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MelomanApp
{

    public class ArtistDialog : Form
    {
        public Artist Result { get; private set; }

        private TextBox txtName, txtGenre, txtCountry, txtYears, txtDesc;

        public ArtistDialog(Artist existing = null)
        {
            Text = existing == null ? "Додати виконавця" : "Редагувати виконавця";
            Size = new Size(420, 310);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;

            int y = 15;
            txtName    = AddField("Назва / Ім'я:",       ref y);
            txtGenre   = AddField("Жанр:",               ref y);
            txtCountry = AddField("Країна:",             ref y);
            txtYears   = AddField("Роки активності:",    ref y);
            txtDesc    = AddField("Опис:",               ref y);

            var btnOk = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Location = new Point(220, y), Size = new Size(80, 28) };
            var btnCn = new Button { Text = "Скасувати", DialogResult = DialogResult.Cancel, Location = new Point(310, y), Size = new Size(90, 28) };
            btnOk.Click += BtnOk_Click;
            Controls.AddRange(new Control[] { btnOk, btnCn });
            AcceptButton = btnOk; CancelButton = btnCn;

            if (existing != null)
            {
                txtName.Text    = existing.Name;
                txtGenre.Text   = existing.Genre;
                txtCountry.Text = existing.Country;
                txtYears.Text   = existing.ActiveYears;
                txtDesc.Text    = existing.Description;
            }
        }

        private TextBox AddField(string label, ref int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(10, y + 3), Size = new Size(150, 20) });
            var tb = new TextBox { Location = new Point(165, y), Size = new Size(230, 22) };
            Controls.Add(tb);
            y += 30;
            return tb;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Введіть назву!"); DialogResult = DialogResult.None; return; }

            Result = new Artist
            {
                Name        = txtName.Text.Trim(),
                Genre       = txtGenre.Text.Trim(),
                Country     = txtCountry.Text.Trim(),
                ActiveYears = txtYears.Text.Trim(),
                Description = txtDesc.Text.Trim()
            };
        }
    }

    public class SongDialog : Form
    {
        public Song Result { get; private set; }

        private TextBox txtTitle, txtDuration, txtYear;
        private ComboBox cmbArtist;
        private readonly List<Artist> _artists;

        public SongDialog(Song existing, List<Artist> artists)
        {
            _artists = artists;
            Text = existing == null ? "Додати пісню" : "Редагувати пісню";
            Size = new Size(420, 240);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;

            int y = 15;
            txtTitle    = AddField("Назва:", ref y);
            AddArtistCombo(ref y);
            txtDuration = AddField("Тривалість (мм:сс):", ref y);
            txtYear     = AddField("Рік:", ref y);

            var btnOk = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Location = new Point(220, y), Size = new Size(80, 28) };
            var btnCn = new Button { Text = "Скасувати", DialogResult = DialogResult.Cancel, Location = new Point(310, y), Size = new Size(90, 28) };
            btnOk.Click += BtnOk_Click;
            Controls.AddRange(new Control[] { btnOk, btnCn });
            AcceptButton = btnOk; CancelButton = btnCn;

            if (existing != null)
            {
                txtTitle.Text    = existing.Title;
                txtDuration.Text = existing.Duration;
                txtYear.Text     = existing.Year.ToString();
                var art = artists.Find(a => a.Id == existing.ArtistId);
                if (art != null) cmbArtist.SelectedItem = art;
            }
        }

        private TextBox AddField(string label, ref int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(10, y + 3), Size = new Size(150, 20) });
            var tb = new TextBox { Location = new Point(165, y), Size = new Size(230, 22) };
            Controls.Add(tb);
            y += 30;
            return tb;
        }

        private void AddArtistCombo(ref int y)
        {
            Controls.Add(new Label { Text = "Виконавець:", Location = new Point(10, y + 3), Size = new Size(150, 20) });
            cmbArtist = new ComboBox { Location = new Point(165, y), Size = new Size(230, 22), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var a in _artists) cmbArtist.Items.Add(a);
            if (cmbArtist.Items.Count > 0) cmbArtist.SelectedIndex = 0;
            Controls.Add(cmbArtist);
            y += 30;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            { MessageBox.Show("Введіть назву!"); DialogResult = DialogResult.None; return; }
            if (!(cmbArtist.SelectedItem is Artist artist))
            { MessageBox.Show("Оберіть виконавця!"); DialogResult = DialogResult.None; return; }

            int.TryParse(txtYear.Text, out int year);
            Result = new Song
            {
                Title    = txtTitle.Text.Trim(),
                ArtistId = artist.Id,
                Duration = txtDuration.Text.Trim(),
                Year     = year
            };
        }
    }

    public class AlbumDialog : Form
    {
        public Album Result { get; private set; }

        private TextBox txtTitle, txtYear, txtLabel;
        private ComboBox cmbArtist;
        private CheckedListBox clbSongs;
        private readonly List<Artist> _artists;
        private readonly List<Song>   _songs;

        public AlbumDialog(Album existing, List<Artist> artists, List<Song> songs)
        {
            _artists = artists; _songs = songs;
            Text = existing == null ? "Додати альбом" : "Редагувати альбом";
            Size = new Size(480, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;

            int y = 15;
            txtTitle = AddField("Назва альбому:", ref y);
            AddArtistCombo(ref y);
            txtYear  = AddField("Рік:", ref y);
            txtLabel = AddField("Лейбл:", ref y);

            Controls.Add(new Label { Text = "Пісні (трек-лист):", Location = new Point(10, y), Size = new Size(200, 20) });
            y += 22;
            clbSongs = new CheckedListBox { Location = new Point(10, y), Size = new Size(445, 280), CheckOnClick = true };
            foreach (var s in songs)
                clbSongs.Items.Add(s, false);
            Controls.Add(clbSongs);
            y += 285;

            var btnOk = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Location = new Point(270, y), Size = new Size(80, 28) };
            var btnCn = new Button { Text = "Скасувати", DialogResult = DialogResult.Cancel, Location = new Point(360, y), Size = new Size(100, 28) };
            btnOk.Click += BtnOk_Click;
            Controls.AddRange(new Control[] { btnOk, btnCn });
            AcceptButton = btnOk; CancelButton = btnCn;

            if (existing != null)
            {
                txtTitle.Text = existing.Title;
                txtYear.Text  = existing.Year.ToString();
                txtLabel.Text = existing.Label;
                var art = artists.Find(a => a.Id == existing.ArtistId);
                if (art != null) cmbArtist.SelectedItem = art;
                for (int i = 0; i < clbSongs.Items.Count; i++)
                    if (existing.SongIds.Contains(((Song)clbSongs.Items[i]).Id))
                        clbSongs.SetItemChecked(i, true);
            }
        }

        private TextBox AddField(string label, ref int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(10, y + 3), Size = new Size(150, 20) });
            var tb = new TextBox { Location = new Point(165, y), Size = new Size(290, 22) };
            Controls.Add(tb); y += 30;
            return tb;
        }

        private void AddArtistCombo(ref int y)
        {
            Controls.Add(new Label { Text = "Виконавець:", Location = new Point(10, y + 3), Size = new Size(150, 20) });
            cmbArtist = new ComboBox { Location = new Point(165, y), Size = new Size(290, 22), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var a in _artists) cmbArtist.Items.Add(a);
            if (cmbArtist.Items.Count > 0) cmbArtist.SelectedIndex = 0;
            Controls.Add(cmbArtist); y += 30;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            { MessageBox.Show("Введіть назву альбому!"); DialogResult = DialogResult.None; return; }
            if (!(cmbArtist.SelectedItem is Artist artist))
            { MessageBox.Show("Оберіть виконавця!"); DialogResult = DialogResult.None; return; }

            int.TryParse(txtYear.Text, out int year);
            var ids = new List<int>();
            foreach (Song s in clbSongs.CheckedItems) ids.Add(s.Id);

            Result = new Album
            {
                Title    = txtTitle.Text.Trim(),
                ArtistId = artist.Id,
                Year     = year,
                Label    = txtLabel.Text.Trim(),
                SongIds  = ids
            };
        }
    }

    public class AlbumTracksDialog : Form
    {
        public AlbumTracksDialog(Album album, DataManager dm)
        {
            Text = $"Треки: {album.Title} ({album.Year})";
            Size = new Size(480, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var lbl = new Label
            {
                Text = $"Виконавець: {dm.GetArtistName(album.ArtistId)}   |   Лейбл: {album.Label}",
                Location = new Point(10, 10), Size = new Size(450, 22),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            var dgv = new DataGridView
            {
                Location = new Point(10, 38), Size = new Size(450, 280),
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White, RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f)
            };
            dgv.Columns.Add("Num",      "#");
            dgv.Columns.Add("Title",    "Назва");
            dgv.Columns.Add("Duration", "Тривалість");
            dgv.Columns.Add("Year",     "Рік");
            dgv.Columns["Num"].Width      = 35;
            dgv.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var songs = dm.GetAlbumSongs(album);
            for (int i = 0; i < songs.Count; i++)
                dgv.Rows.Add(i + 1, songs[i].Title, songs[i].Duration, songs[i].Year);

            var btnClose = new Button
            {
                Text = "Закрити", DialogResult = DialogResult.Cancel,
                Location = new Point(370, 325), Size = new Size(90, 28)
            };

            Controls.AddRange(new Control[] { lbl, dgv, btnClose });
            CancelButton = btnClose;
        }
    }
}
