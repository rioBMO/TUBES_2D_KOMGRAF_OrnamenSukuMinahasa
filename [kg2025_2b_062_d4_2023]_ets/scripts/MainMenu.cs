using Godot;
using System;

public partial class MainMenu : Control
{
	private ColorRect topRectangle;
	private ColorRect bottomRectangle;
	private bool profileVisible = false;
	private Vector2 originalTopPosition; // Posisi asli persegi atas
	private Vector2 originalBottomPosition; // Posisi asli persegi bawah
	private TextureRect iconDisplay; // Untuk menampilkan icon yang berubah-ubah
	private Label iconTextLabel; // Tambahkan field baru untuk Label teks
								 // Tambahkan variabel baru untuk menyimpan label-label informasi profil
	private Label[] profileLabels = new Label[4];
	private FontFile iconFont; // Tambahkan ini - deklarasi iconFont sebagai field kelas

	// Tambahkan variabel member baru untuk persegi guide
	private ColorRect guideTopRectangle;
	private ColorRect guideBottomRectangle;
	private bool guideVisible = false;
	private Vector2 originalGuideTopPosition;
	private Vector2 originalGuideBottomPosition;

	// Tambahkan variabel kelas untuk menyimpan referensi ke label guide
	private Label guideLabel;

	// 1. Tambahkan variabel kelas baru
	private ColorRect guideMiddleRectangle;
	private Vector2 originalGuideMiddlePosition;

	// Tambahkan variabel baru untuk menyimpan label instruksi
	private Label guideInstructionsLabel;

	// Modifikasi PlayButtonSound untuk menggunakan delay
	private void PlayButtonSound(string buttonName, Action callback = null)
	{
		// Durasi suara button-click adalah 0.34 detik
		float soundDuration = 0.34f;
		bool soundPlayed = false;

		// Coba cari node AudioStreamPlayer2D secara langsung
		var audioPlayer = GetNode<AudioStreamPlayer2D>($"{buttonName}/AudioStreamPlayer2D");

		if (audioPlayer == null)
		{
			// Jika tidak ditemukan, coba gunakan AudioStreamPlayer yang sama untuk semua tombol
			audioPlayer = GetNode<AudioStreamPlayer2D>("ButtonClickSound");

			// Jika masih null, coba cari di parent
			if (audioPlayer == null)
			{
				// Coba cari di root
				audioPlayer = GetTree().Root.GetNode<AudioStreamPlayer2D>("ButtonClickSound");
			}
		}

		// Pastikan audioPlayer ditemukan dan putar suaranya
		if (audioPlayer != null)
		{
			audioPlayer.Play();
			soundPlayed = true;
		}
		else
		{
			// Jika semua cara gagal, buat AudioStreamPlayer baru secara dinamis
			var tempPlayer = new AudioStreamPlayer();
			AddChild(tempPlayer);

			// Coba path yang benar ke file suara
			AudioStream clickSound = null;

			// Coba beberapa kemungkinan path file suara
			string[] possiblePaths = new string[] {
				"res://assets/sounds/button-click.mp3",  // Path yang benar dari prompt
				"res://assets/sounds/button_click.mp3",  // Alternatif dengan underscore
				"res://assets/sounds/button-click.wav",  // Alternatif ekstensi wav
				"res://assets/sounds/button_click.wav",  // Path original
			};

			foreach (var path in possiblePaths)
			{
				clickSound = GD.Load<AudioStream>(path);
				if (clickSound != null)
				{
					GD.Print($"Berhasil memuat suara dari: {path}");
					break;
				}
			}

			if (clickSound != null)
			{
				tempPlayer.Stream = clickSound;
				tempPlayer.Play();
				soundPlayed = true;

				// Hapus node setelah selesai memutar
				tempPlayer.Finished += () => tempPlayer.QueueFree();
			}
			else
			{
				GD.Print($"Tidak bisa memuat file suara tombol. Path yang dicoba: {string.Join(", ", possiblePaths)}");
				// Jika tidak ada suara, langsung jalankan callback
				callback?.Invoke();
				return;
			}
		}

		// Jika suara berhasil diputar dan ada callback, tunggu sebentar sebelum melanjutkan
		if (soundPlayed && callback != null)
		{
			var timer = GetTree().CreateTimer(soundDuration);
			timer.Timeout += () => callback();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Membuat persegi pertama (atas)
		topRectangle = new ColorRect();
		topRectangle.Size = new Vector2(510, 595);
		topRectangle.Color = new Color("#FBFBFB", 0.9f); // Warna #FBFBFB dengan transparansi
		AddChild(topRectangle);

		// Menambahkan garis horizontal pada persegi pertama
		ColorRect horizontalLine = new ColorRect();
		horizontalLine.Size = new Vector2(442, 2); // Panjang 442 pixel dan tinggi 2 pixel

		// Posisikan garis 91 pixel dari titik awal persegi dan ditengah secara horizontal
		float centeredX = (topRectangle.Size.X - horizontalLine.Size.X) / 2; // Posisi tengah horizontal
		horizontalLine.Position = new Vector2(centeredX, 91);

		// Beri warna gelap pada garis
		horizontalLine.Color = new Color("#333333");

		// Tambahkan garis sebagai anak dari persegi pertama
		topRectangle.AddChild(horizontalLine);

		// Tambahkan gambar profil
		TextureRect profileImage = new TextureRect();
		profileImage.Texture = GD.Load<Texture2D>("res://assets/ui/profil.png");

		if (profileImage.Texture != null)
		{
			// Atur ukuran gambar
			profileImage.CustomMinimumSize = new Vector2(213, 384);
			profileImage.Size = new Vector2(213, 384);
			profileImage.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;

			// Hitung posisi gambar (di tengah persegi dan 15 pixel di bawah garis)
			// Gunakan nama variabel berbeda untuk menghindari konflik
			float imageCenteredX = (topRectangle.Size.X - profileImage.Size.X) / 2;
			float positionY = horizontalLine.Position.Y + horizontalLine.Size.Y + 15;

			// Atur posisi gambar
			profileImage.Position = new Vector2(imageCenteredX, positionY);

			// Tambahkan gambar ke persegi pertama
			topRectangle.AddChild(profileImage);

			// Tambahkan tiga tombol icon di bawah gambar profil
			// Hitung posisi Y untuk tombol (28 pixel di bawah gambar profil)
			float buttonY = positionY + profileImage.Size.Y + 28;

			// Ukuran tombol (asumsikan semua tombol memiliki ukuran yang sama)
			int buttonSize = 50; // Sesuaikan dengan ukuran aktual gambar icon

			// Hitung total lebar yang dibutuhkan untuk tiga tombol (jarak antar tombol 91 pixel)
			float totalWidth = (3 * buttonSize) + (2 * 91);

			// Hitung posisi X awal agar tombol-tombol berada di tengah
			float startX = (topRectangle.Size.X - totalWidth) / 2;

			// Buat dan tambahkan tiga tombol
			string[] iconPaths = {
				"res://assets/ui/Man-Ringed.png",
				"res://assets/ui/Quest-Ringed.png",
				"res://assets/ui/Location-Ringed.png"
			};

			for (int i = 0; i < 3; i++)
			{
				// Buat tombol
				TextureButton iconButton = new TextureButton();

				// Muat tekstur tombol normal dan active
				string normalPath = iconPaths[i];
				string activePath = normalPath.Replace(".png", "-Active.png");

				var textureNormal = GD.Load<Texture2D>(normalPath);
				var textureActive = GD.Load<Texture2D>(activePath);

				if (textureNormal != null && textureActive != null)
				{
					// Atur tekstur untuk berbagai state tombol dengan nama properti yang benar
					iconButton.TextureNormal = textureNormal;
					iconButton.TextureHover = textureActive;     // Nama properti yang benar
					iconButton.TexturePressed = textureActive;
					iconButton.TextureFocused = textureActive;

					// Toggle mode untuk mempertahankan state aktif saat diklik
					iconButton.ToggleMode = true;
					iconButton.TextureDisabled = textureNormal;

					// Properti lain
					iconButton.IgnoreTextureSize = false;
					iconButton.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;
					iconButton.Size = new Vector2(buttonSize, buttonSize);

					// Hitung posisi X untuk tombol ini
					float buttonX = startX + (i * (buttonSize + 91));
					iconButton.Position = new Vector2(buttonX, buttonY);

					// Tambahkan tombol ke persegi
					topRectangle.AddChild(iconButton);

					// Sambungkan sinyal tombol untuk memutar suara
					iconButton.Pressed += () =>
					{
						PlayButtonSound($"iconButton{i}");
					};

					// Tambahkan event handler untuk button toggling
					int buttonIndex = i; // Capture current index
					iconButton.Toggled += (toggled) =>
					{
						if (toggled)
						{
							// Nonaktifkan tombol lain dengan cara yang lebih aman
							for (int j = 0; j < topRectangle.GetChildCount(); j++)
							{
								var child = topRectangle.GetChild(j);
								if (child is TextureButton otherButton && otherButton != iconButton && otherButton.ButtonPressed)
								{
									otherButton.ButtonPressed = false;
								}
							}

							// Ubah icon yang ditampilkan berdasarkan tombol yang ditekan
							string iconName = "";
							switch (buttonIndex)
							{
								case 0:
									iconName = "Man";
									break;
								case 1:
									iconName = "Quest";
									break;
								case 2:
									iconName = "Location";
									break;
							}

							// Debug output untuk memverifikasi bahwa kode ini dijalankan
							GD.Print($"Tombol {buttonIndex} dipilih, menampilkan icon: {iconName}");

							// Panggil metode untuk mengubah icon
							ChangeDisplayedIcon(iconName);
						}
					};
				}
				else
				{
					GD.Print($"Tidak dapat memuat gambar tombol: {normalPath} atau {activePath}");
				}
			}
		}
		else
		{
			GD.Print("Tidak dapat memuat gambar profil: res://assets/ui/profil.png");
		}

		// Tambahkan teks "rioBMO" di antara titik awal persegi dan garis
		Label rioLabel = new Label();
		rioLabel.Text = "rioBMO";
		rioLabel.HorizontalAlignment = HorizontalAlignment.Center;

		// Muat font kustom - perbaiki path font
		var fontPath = "res://assets/fonts/SAOUITT-Regular.ttf"; // Hapus .import di akhir
		var customFont = GD.Load<FontFile>(fontPath);

		// Tambahkan fallback jika font masih tidak ditemukan
		if (customFont == null)
		{
			// Coba path alternatif
			fontPath = "res://fonts/SAOUITT-Regular.ttf";
			customFont = GD.Load<FontFile>(fontPath);

			if (customFont == null)
			{
				GD.Print("Font tidak ditemukan: " + fontPath);
				// Gunakan font default sebagai fallback
				rioLabel.AddThemeFontSizeOverride("font_size", 37);
			}
		}

		// Terapkan font kustom ke label
		if (customFont != null)
		{
			rioLabel.AddThemeFontOverride("font", customFont);
			rioLabel.AddThemeFontSizeOverride("font_size", 37); // Sesuaikan ukuran font sesuai kebutuhan
		}
		else
		{
			GD.Print("Font tidak ditemukan: " + fontPath);
		}

		// Posisikan label di antara awal persegi dan garis (sekitar 45 pixel dari atas)
		rioLabel.Position = new Vector2(0, 45);
		rioLabel.Size = new Vector2(topRectangle.Size.X, 40);

		// Atur warna teks menjadi gelap
		rioLabel.AddThemeColorOverride("font_color", new Color("#333333"));
		// Untuk label rioBMO
		rioLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));

		// Tambahkan label sebagai child dari persegi pertama
		topRectangle.AddChild(rioLabel);

		// Membuat persegi kedua (bawah)
		bottomRectangle = new ColorRect();
		bottomRectangle.Size = new Vector2(510, 295);
		bottomRectangle.Color = new Color("#D7D7D7", 0.9f); // Warna #D7D7D7 dengan transparansi
		AddChild(bottomRectangle);

		// Tambahkan icon display pada persegi kedua
		iconDisplay = new TextureRect();
		iconDisplay.Position = new Vector2(36, 30); // 36px dari kiri, 30px dari atas
		iconDisplay.Size = new Vector2(52.5f, 52.5f); // Ukuran icon diubah menjadi 52,5 pixel
		iconDisplay.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize; // Menggunakan nilai yang sudah bekerja di profileImage

		// Jangan set icon default - biarkan kosong sampai tombol diklik
		iconDisplay.Texture = null; // Set texture kosong

		// Tambahkan icon ke persegi kedua
		bottomRectangle.AddChild(iconDisplay);

		// Tambahkan label untuk teks di sebelah icon
		iconTextLabel = new Label();
		iconTextLabel.Position = new Vector2(36 + 52.5f + 15, 30); // 15px dari kanan icon
		iconTextLabel.Size = new Vector2(bottomRectangle.Size.X - (36 + 52.5f + 30), 52.5f);
		iconTextLabel.VerticalAlignment = VerticalAlignment.Center;

		// Muat dan gunakan font yang sama (dalam _Ready())
		var iconFontPath = "res://assets/fonts/SAOUITT-Regular.ttf";
		iconFont = GD.Load<FontFile>(iconFontPath); // Hapus 'var' karena sekarang kita menggunakan field kelas

		// Jika font tidak ditemukan di path pertama, coba path kedua
		if (iconFont == null)
		{
			iconFontPath = "res://fonts/SAOUITT-Regular.ttf";
			iconFont = GD.Load<FontFile>(iconFontPath);
		}

		// Terapkan font jika berhasil dimuat
		if (iconFont != null)
		{
			iconTextLabel.AddThemeFontOverride("font", iconFont);
			iconTextLabel.AddThemeFontSizeOverride("font_size", 33); // Changed from 33.4f to 33
		}

		// Warna teks gelap
		iconTextLabel.AddThemeColorOverride("font_color", new Color("#333333"));
		// Untuk label di sebelah icon
		iconTextLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));

		// Default teks kosong
		iconTextLabel.Text = "";

		// Tambahkan label ke persegi kedua
		bottomRectangle.AddChild(iconTextLabel);

		// Gunakan posisi default yang sudah ditentukan
		topRectangle.Position = new Vector2(50, 50);
		bottomRectangle.Position = new Vector2(50, 50 + topRectangle.Size.Y);

		// Simpan posisi asli untuk referensi
		originalTopPosition = topRectangle.Position;
		originalBottomPosition = bottomRectangle.Position;

		// Sembunyikan persegi saat startup
		topRectangle.Visible = false;
		bottomRectangle.Visible = false;

		// Membuat persegi untuk guide (bagian kanan)
		guideTopRectangle = new ColorRect();
		guideTopRectangle.Size = new Vector2(510, 91); // Seharusnya sudah benar
		guideTopRectangle.Color = new Color("#F9F9F9");
		AddChild(guideTopRectangle);

		guideBottomRectangle = new ColorRect();
		guideBottomRectangle.Size = new Vector2(510, 101); // Seharusnya sudah benar
		guideBottomRectangle.Color = new Color("#FFFFFF");
		AddChild(guideBottomRectangle);

		// Hitung posisi persegi guide
		// Gunakan ukuran viewport untuk menempatkan di bagian kanan
		float screenWidth = GetViewportRect().Size.X;
		float rightColumnX = screenWidth - 50 - 510; // 50px dari tepi kanan layar
		float verticalMiddleY = 50; // Posisi Y yang sama dengan persegi profil

		guideTopRectangle.Position = new Vector2(rightColumnX, verticalMiddleY);
		guideBottomRectangle.Position = new Vector2(rightColumnX, verticalMiddleY + guideTopRectangle.Size.Y);

		// Simpan posisi asli untuk referensi
		originalGuideTopPosition = guideTopRectangle.Position;
		originalGuideBottomPosition = guideBottomRectangle.Position;

		// Tambahkan teks "Guide" di tengah persegi guide pertama
		guideLabel = new Label();
		guideLabel.Text = "Guide";
		guideLabel.HorizontalAlignment = HorizontalAlignment.Center;
		guideLabel.ClipText = true; // Tambahkan ini untuk memastikan teks terpotong sesuai lebar persegi

		// Gunakan font yang sama dengan rioLabel
		if (customFont != null)
		{
			guideLabel.AddThemeFontOverride("font", customFont);
			guideLabel.AddThemeFontSizeOverride("font_size", 37); // Ukuran yang sama dengan rioLabel
		}

		// Posisikan label di tengah persegi guide pertama (vertikal dan horizontal)
		guideLabel.Position = new Vector2(0, (guideTopRectangle.Size.Y - 40) / 2); // Tengah vertikal
		guideLabel.Size = new Vector2(guideTopRectangle.Size.X, 40);

		// Atur warna teks sama dengan rioLabel
		guideLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));

		// Set pivot di tengah untuk animasi scale yang benar dari tengah
		guideLabel.PivotOffset = new Vector2(guideLabel.Size.X / 2, guideLabel.Size.Y / 2);
		guideLabel.GrowHorizontal = Control.GrowDirection.Both;
		guideLabel.GrowVertical = Control.GrowDirection.Both;

		// Tambahkan label sebagai child dari persegi guide pertama
		guideTopRectangle.AddChild(guideLabel);

		// Sembunyikan persegi saat awal
		guideTopRectangle.Visible = false;
		guideBottomRectangle.Visible = false;

		// Membuat persegi ketiga untuk guide (di tengah)
		guideMiddleRectangle = new ColorRect();
		// Increase height from 350 to 450 to provide more space for the instruction text
		guideMiddleRectangle.Size = new Vector2(510, 450);
		guideMiddleRectangle.Color = new Color("#D8D8D8");
		AddChild(guideMiddleRectangle);

		// Tambahkan teks instruksi pada persegi tengah guide
		guideInstructionsLabel = new Label();
		guideInstructionsLabel.Text =
		@"Instructions for Karya 4:

		Controls:
		R - Reverse Orbit Rotation
		C - Swap Eye / Cross Colors
		+ - Increase Animation Speed
		- - Decrease Animation Speed

		Kolintang Motif:
		- Click arrow button to switch to kolintang
		- Click on wooden bars to play notes
		- Press A to toggle auto-play mode

		Press H to hide instructions";

		// Atur properti font
		if (iconFont != null) // Gunakan iconFont yang sudah ada
		{
			guideInstructionsLabel.AddThemeFontOverride("font", iconFont);
			guideInstructionsLabel.AddThemeFontSizeOverride("font_size", 24);
		}

		// Atur horizontal alignment ke tengah
		guideInstructionsLabel.HorizontalAlignment = HorizontalAlignment.Center;

		// Atur vertical alignment ke tengah
		guideInstructionsLabel.VerticalAlignment = VerticalAlignment.Center;

		// Atur posisi dan ukuran label untuk mengisi seluruh persegi ketiga
		guideInstructionsLabel.Position = new Vector2(0, 0); // Mulai dari pojok kiri atas persegi
		guideInstructionsLabel.Size = new Vector2(guideMiddleRectangle.Size.X, guideMiddleRectangle.Size.Y);

		// Atur warna teks yang konsisten dengan label lain
		guideInstructionsLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));

		// Atur posisi dan ukuran label
		guideInstructionsLabel.Position = new Vector2(20, 20); // Margin dari tepi persegi
		guideInstructionsLabel.Size = new Vector2(guideMiddleRectangle.Size.X - 40, guideMiddleRectangle.Size.Y - 40);

		// Atur warna teks yang konsisten dengan label lain
		guideInstructionsLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));

		// Tambahkan label ke persegi ketiga
		guideMiddleRectangle.AddChild(guideInstructionsLabel);

		// Sembunyikan label saat awal
		guideInstructionsLabel.Visible = true; // Akan mengikuti visibilitas parent (guideMiddleRectangle)

		// Sembunyikan persegi saat awal
		guideMiddleRectangle.Visible = false;

		// Add "No" button to the guide's bottom rectangle
		TextureButton noButton = new TextureButton();

		// Load button textures
		var noTexture = GD.Load<Texture2D>("res://assets/ui/No.png");
		var noOnTexture = GD.Load<Texture2D>("res://assets/ui/No_on.png");

		if (noTexture != null && noOnTexture != null)
		{
			// Set button textures for different states
			noButton.TextureNormal = noTexture;
			noButton.TextureHover = noOnTexture;
			noButton.TexturePressed = noOnTexture;
			noButton.TextureFocused = noOnTexture;

			// Keep natural size of the texture
			noButton.IgnoreTextureSize = true;  // Ubah ke true agar kita bisa mengatur ukuran secara eksplisit
			noButton.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;

			// Atur ukuran tombol menjadi 42px x 42px
			noButton.CustomMinimumSize = new Vector2(42, 42);
			noButton.Size = new Vector2(42, 42);

			// Position in center of the second rectangle
			float centerX = (guideBottomRectangle.Size.X - noButton.Size.X) / 2;
			float centerY = (guideBottomRectangle.Size.Y - noButton.Size.Y) / 2;
			noButton.Position = new Vector2(centerX, centerY); // Posisi di tengah persegi kedua

			// Connect button press signal to close the guide
			noButton.Pressed += () =>
			{
				// Play button sound first
				PlayButtonSound("ButtonClickSound");

				// Close guide after sound finishes
				GetTree().CreateTimer(0.34).Timeout += () =>
				{
					// Set guideVisible to false
					guideVisible = false;

					// Tambahkan ini: Putar suara objek menghilang
					PlayObjectAppearSound();

					// Sembunyikan SEMUA teks dan tombol terlebih dahulu sebelum animasi
					guideLabel.Visible = false;
					guideInstructionsLabel.Visible = false;  // Tambahkan baris ini agar teks instruksi juga langsung menghilang

					// Sembunyikan diri sendiri juga
					noButton.Visible = false;

					// Simpan posisi tengah untuk titik akhir animasi
					float centerX = originalGuideBottomPosition.X + 510 / 2;

					// Animasi untuk menyembunyikan persegi
					var tween = CreateTween();

					// 1. Persegi tengah mengecil vertikal terlebih dahulu
					tween.TweenProperty(guideMiddleRectangle, "size:y", 0, 0.3)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

					// Bersamaan, geser persegi bawah ke atas (kembali ke posisi asli)
					tween.Parallel().TweenProperty(guideBottomRectangle, "position:y",
									originalGuideTopPosition.Y + guideTopRectangle.Size.Y, 0.3)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

					// Tunggu sebentar
					tween.TweenInterval(0.1);

					// 2. Persegi atas dan bawah mengecil secara bersamaan (horizontal)
					tween.Parallel().TweenProperty(guideTopRectangle, "size:x", 0, 0.4)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
					tween.Parallel().TweenProperty(guideTopRectangle, "position:x", centerX, 0.4)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

					tween.Parallel().TweenProperty(guideBottomRectangle, "size:x", 0, 0.4)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
					tween.Parallel().TweenProperty(guideBottomRectangle, "position:x", centerX, 0.4)
						 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

					// Sembunyikan ketiga persegi setelah animasi selesai
					tween.TweenCallback(Callable.From(() =>
					{
						guideTopRectangle.Visible = false;
						guideBottomRectangle.Visible = false;
						guideMiddleRectangle.Visible = false;

						// Reset ukuran dan posisi asli ketiga persegi
						guideTopRectangle.Size = new Vector2(510, 91);
						guideBottomRectangle.Size = new Vector2(510, 101);
						guideMiddleRectangle.Size = new Vector2(510, 450);
						guideTopRectangle.Position = originalGuideTopPosition;
						guideBottomRectangle.Position = originalGuideBottomPosition;
						guideMiddleRectangle.Position = originalGuideMiddlePosition;
					}));
				};
			};

			// Add the button to the guide bottom rectangle
			guideBottomRectangle.AddChild(noButton);

			// Sembunyikan tombol No secara default (seperti teks Guide)
			noButton.Visible = false;
		}
		else
		{
			GD.Print("Failed to load No button textures: res://assets/ui/No.png or res://assets/ui/No_on.png");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Modifikasi handler tombol untuk menggunakan callback
	private void _on_karya1Btn_pressed()
	{
		PlayButtonSound("karya1Btn", () => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D.tscn"));
	}

	private void _on_karya2Btn_pressed()
	{
		PlayButtonSound("karya2Btn", () => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Animasi.tscn"));
	}

	private void _on_karya3Btn_pressed()
	{
		PlayButtonSound("karya3Btn", () => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Polygon_Animasi.tscn"));
	}

	private void _on_karya4Btn_pressed()
	{
		PlayButtonSound("karya4Btn", () => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Animasi_dan_Interaksi.tscn"));
	}

	private void _on_guideBtn_pressed()
	{
		// Putar suara tombol terlebih dahulu
		PlayButtonSound("guideBtn");

		// Delay untuk menunggu suara button-click selesai (0.34 detik)
		GetTree().CreateTimer(0.34).Timeout += () =>
		{
			// Toggle visibility ketika tombol guide ditekan
			guideVisible = !guideVisible;

			if (guideVisible)
			{
				// Putar suara objek muncul
				PlayObjectAppearSound();

				// Reset persegi ke posisi asli terlebih dahulu untuk ukuran tinggi
				guideTopRectangle.Position = originalGuideTopPosition;
				guideBottomRectangle.Position = originalGuideBottomPosition;
				guideTopRectangle.Size = new Vector2(510, 91);
				guideBottomRectangle.Size = new Vector2(510, 101);

				// Sembunyikan teks Guide terlebih dahulu, akan dimunculkan setelah animasi selesai
				guideLabel.Visible = false;  // Ubah ini menjadi false

				// Simpan posisi tengah untuk titik animasi
				float centerX = originalGuideTopPosition.X + 510 / 2;

				// Siapkan kedua persegi untuk animasi melebar ke samping
				guideTopRectangle.Size = new Vector2(0, 91);
				guideBottomRectangle.Size = new Vector2(0, 101);

				guideTopRectangle.Position = new Vector2(centerX, originalGuideTopPosition.Y);
				guideBottomRectangle.Position = new Vector2(centerX, originalGuideBottomPosition.Y);

				// Tampilkan kedua persegi sejak awal
				guideTopRectangle.Visible = true;
				guideBottomRectangle.Visible = true;

				// Buat tween sequence untuk animasi bersamaan
				var tween = CreateTween();

				// Animasi kedua persegi melebar secara bersamaan
				// Persegi atas
				tween.Parallel().TweenProperty(guideTopRectangle, "size:x", 510, 0.5)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
				tween.Parallel().TweenProperty(guideTopRectangle, "position:x", originalGuideTopPosition.X, 0.5)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

				// Persegi bawah
				tween.Parallel().TweenProperty(guideBottomRectangle, "size:x", 510, 0.5)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
				tween.Parallel().TweenProperty(guideBottomRectangle, "position:x", originalGuideBottomPosition.X, 0.5)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

				// Di akhir animasi, tambahkan callback untuk menampilkan teks "Guide"
				tween.TweenCallback(Callable.From(() =>
				{
					guideLabel.Visible = true; // Munculkan teks setelah animasi selesai

					// Persiapkan persegi tengah untuk animasi
					guideMiddleRectangle.Size = new Vector2(510, 0); // Tinggi awal 0
					guideMiddleRectangle.Position = new Vector2(
						originalGuideTopPosition.X,
						originalGuideTopPosition.Y + guideTopRectangle.Size.Y
					);
					guideMiddleRectangle.Visible = true;
					guideInstructionsLabel.Visible = true; // Tambahkan baris ini untuk memastikan teks instruksi muncul kembali

					// Tambahkan ini: Munculkan tombol No setelah animasi persegi selesai
					// Mencari tombol No di children dari guideBottomRectangle
					foreach (var child in guideBottomRectangle.GetChildren())
					{
						if (child is TextureButton button)
						{
							button.Visible = true;
						}
					}

					// TAMBAHKAN KODE INI: Buat tween baru untuk animasi persegi tengah
					var middleTween = CreateTween();

					// Animasi persegi tengah memanjang ke bawah - update to new height (450)
					middleTween.TweenProperty(guideMiddleRectangle, "size:y", 450, 0.4)
							.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

					// Bersamaan, geser persegi bawah ke bawah seiring dengan pemanjangan persegi tengah - update to new height (450)
					middleTween.Parallel().TweenProperty(guideBottomRectangle, "position:y",
									originalGuideTopPosition.Y + guideTopRectangle.Size.Y + 450, 0.4)
							.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
				}));
			}
			else // Saat persegi guide disembunyikan
			{
				// Putar suara objek menghilang
				PlayObjectAppearSound();

				// Sembunyikan SEMUA teks terlebih dahulu sebelum animasi
				guideLabel.Visible = false;
				guideInstructionsLabel.Visible = false;  // Tambahkan ini untuk menghilangkan teks di persegi tengah terlebih dahulu

				// Tambahkan ini: Sembunyikan tombol No
				foreach (var child in guideBottomRectangle.GetChildren())
				{
					if (child is TextureButton button)
					{
						button.Visible = false;
					}
				}

				// Simpan posisi tengah untuk titik akhir animasi
				float centerX = originalGuideBottomPosition.X + 510 / 2;

				// Animasi untuk menyembunyikan persegi
				var tween = CreateTween();

				// 1. Persegi tengah mengecil vertikal terlebih dahulu
				tween.TweenProperty(guideMiddleRectangle, "size:y", 0, 0.3)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// Bersamaan, geser persegi bawah ke atas (kembali ke posisi asli)
				tween.Parallel().TweenProperty(guideBottomRectangle, "position:y",
								originalGuideTopPosition.Y + guideTopRectangle.Size.Y, 0.3)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// Tunggu sebentar
				tween.TweenInterval(0.1);

				// 2. Persegi atas dan bawah mengecil secara bersamaan (horizontal)
				// Persegi atas
				tween.Parallel().TweenProperty(guideTopRectangle, "size:x", 0, 0.4)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
				tween.Parallel().TweenProperty(guideTopRectangle, "position:x", centerX, 0.4)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// Persegi bawah
				tween.Parallel().TweenProperty(guideBottomRectangle, "size:x", 0, 0.4)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
				tween.Parallel().TweenProperty(guideBottomRectangle, "position:x", centerX, 0.4)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// 3. Sembunyikan persegi dan reset properti
				tween.TweenCallback(Callable.From(() =>
				{
					guideTopRectangle.Visible = false;
					guideBottomRectangle.Visible = false;
					guideMiddleRectangle.Visible = false;

					// Reset ukuran dan posisi asli ketiga persegi
					guideTopRectangle.Size = new Vector2(510, 91);
					guideBottomRectangle.Size = new Vector2(510, 101);
					guideMiddleRectangle.Size = new Vector2(510, 450);
					guideTopRectangle.Position = originalGuideTopPosition;
					guideBottomRectangle.Position = originalGuideBottomPosition;
					guideMiddleRectangle.Position = originalGuideMiddlePosition;
				}));
			}
		};
	}

	private void _on_aboutBtn_pressed()
	{
		// Putar suara tombol terlebih dahulu
		PlayButtonSound("aboutBtn");

		// Delay untuk menunggu suara button-click selesai (0.34 detik)
		GetTree().CreateTimer(0.34).Timeout += () =>
		{
			// Toggle visibility ketika tombol about ditekan
			profileVisible = !profileVisible;

			if (profileVisible)
			{
				// Putar suara objek muncul
				PlayObjectAppearSound();

				// Reset persegi ke posisi asli terlebih dahulu
				topRectangle.Position = originalTopPosition;
				bottomRectangle.Position = originalBottomPosition;

				// Aktifkan persegi atas agar terlihat
				topRectangle.Visible = true;
				// Sembunyikan persegi bawah dahulu
				bottomRectangle.Visible = false;

				// Simpan posisi akhir yang diinginkan dan ukuran asli
				Vector2 finalTopPosition = originalTopPosition;
				Vector2 finalBottomPosition = originalBottomPosition;
				float finalTopHeight = 595; // Ukuran asli tinggi persegi atas

				// Siapkan persegi atas untuk animasi memanjang (tinggi 0)
				topRectangle.Size = new Vector2(topRectangle.Size.X, 0);

				// Buat tween sequence
				var tween = CreateTween();

				// 1. Animasi persegi atas memanjang dari atas ke bawah
				tween.TweenProperty(topRectangle, "size:y", finalTopHeight, 0.5)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

				// 2. Tunggu persegi atas selesai
				tween.TweenInterval(0.2);

				// 3. Setelah persegi atas selesai, posisikan persegi bawah tepat di bawah persegi atas
				// tapi dengan tinggi 0 (untuk efek memanjang)
				tween.TweenCallback(Callable.From(() =>
				{
					bottomRectangle.Visible = true;
					// Posisikan di bawah persegi atas dengan tinggi 0
					bottomRectangle.Position = new Vector2(finalBottomPosition.X, finalTopPosition.Y + topRectangle.Size.Y);
					bottomRectangle.Size = new Vector2(bottomRectangle.Size.X, 0);
				}));

				// 4. Animasi persegi bawah "memanjang" ke bawah
				tween.TweenProperty(bottomRectangle, "size:y", 295, 0.4)
					 .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
			}
			else // Saat persegi profil disembunyikan
			{
				// Putar suara objek menghilang
				PlayObjectAppearSound();

				// PERTAMA: Sembunyikan teks icon dan teks informasi di persegi bawah terlebih dahulu
				iconTextLabel.Visible = false;

				// Sembunyikan semua label profil (teks informasi)
				for (int i = 0; i < profileLabels.Length; i++)
				{
					if (profileLabels[i] != null)
					{
						profileLabels[i].Visible = false;
					}
				}

				// KEDUA: Baru sembunyikan gambar profil dan tombol-tombol di persegi atas
				// Iterasi semua child dalam topRectangle
				for (int i = 0; i < topRectangle.GetChildCount(); i++)
				{
					var child = topRectangle.GetChild(i);

					// Sembunyikan semua child KECUALI label "rioBMO" dan garis horizontal
					if (child is CanvasItem canvasItem)
					{
						// Jika bukan label "rioBMO" dan bukan garis horizontal
						if (!(child is Label label && label.Text == "rioBMO") &&
							!(child is ColorRect rect && rect.Size.Y == 2)) // Garis horizontal memiliki tinggi 2px
						{
							canvasItem.Visible = false;
						}
					}
				}

				// Animasi untuk menyembunyikan persegi
				var tween = CreateTween();

				// 1. Persegi bawah mengecil
				tween.TweenProperty(bottomRectangle, "size:y", 0, 0.3)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// 2. Setelah persegi bawah menghilang, sembunyikan
				tween.TweenCallback(Callable.From(() =>
				{
					bottomRectangle.Visible = false;
				}));

				// 3. Tunggu sebentar
				tween.TweenInterval(0.1);

				// 4. Persegi atas mengecil
				tween.TweenProperty(topRectangle, "size:y", 0, 0.4)
					 .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

				// 5. Sembunyikan persegi atas dan reset properti untuk klik berikutnya
				tween.TweenCallback(Callable.From(() =>
				{
					topRectangle.Visible = false;

					// Reset ukuran asli kedua persegi
					topRectangle.Size = new Vector2(510, 595);
					bottomRectangle.Size = new Vector2(510, 295);

					// Reset visibilitas semua child saat sembunyi
					// 1. Reset child dari topRectangle
					for (int i = 0; i < topRectangle.GetChildCount(); i++)
					{
						if (topRectangle.GetChild(i) is CanvasItem canvasItem)
						{
							canvasItem.Visible = true;
						}
					}

					// 2. Tambahkan ini: Reset visibilitas untuk iconTextLabel
					iconTextLabel.Visible = true;

					// 3. Tambahkan ini: Reset visibilitas untuk semua profileLabels
					for (int i = 0; i < profileLabels.Length; i++)
					{
						if (profileLabels[i] != null)
						{
							profileLabels[i].Visible = true;
						}
					}
				}));
			}
		};
	}

	private void _on_exitBtn_pressed()
	{
		PlayButtonSound("exitBtn", () => GetTree().Quit());
	}

	// Tambahkan method baru untuk memutar suara objek muncul
	private void PlayObjectAppearSound()
	{
		// Buat AudioStreamPlayer baru
		var appearSound = new AudioStreamPlayer();
		AddChild(appearSound);

		// Coba load suara
		var sound = GD.Load<AudioStream>("res://assets/sounds/object-appear.mp3");
		if (sound != null)
		{
			appearSound.Stream = sound;
			appearSound.Play();

			// Hapus node setelah selesai diputar
			appearSound.Finished += () => appearSound.QueueFree();
		}
		else
		{
			GD.Print("Tidak bisa memuat suara object-appear.mp3");
			appearSound.QueueFree();
		}
	}

	// Modifikasi fungsi ChangeDisplayedIcon
	private void ChangeDisplayedIcon(string iconName)
	{
		GD.Print($"ChangeDisplayedIcon dipanggil dengan: {iconName}");

		// Path ke icon berdasarkan nama
		string iconPath = $"res://assets/ui/{iconName}.png";
		GD.Print($"Mencoba memuat icon dari: {iconPath}");

		// Coba muat icon baru
		var newTexture = GD.Load<Texture2D>(iconPath);

		if (newTexture != null)
		{
			GD.Print($"Berhasil memuat icon: {iconPath}");

			// Pastikan persegi kedua terlihat
			bottomRectangle.Visible = true;

			// Tampilkan icon
			iconDisplay.Texture = newTexture;

			// Hapus semua label profil lama terlebih dahulu
			for (int i = 0; i < profileLabels.Length; i++)
			{
				if (profileLabels[i] != null)
				{
					profileLabels[i].QueueFree();
					profileLabels[i] = null;
				}
			}

			// Set teks sesuai dengan icon yang dipilih
			switch (iconName)
			{
				case "Man":
					iconTextLabel.Text = "Profile";

					// Tunggu sebentar untuk memastikan label lama sudah dihapus
					GetTree().CreateTimer(0.05).Timeout += () =>
					{
						// Tambahkan informasi profil di bawah text "Profile"
						string[] profileInfo = {
							"Nama  :  Satryo Haryo Bimo",
							"NIM     : 231524062",
							"Kelas  : 2B",
							"Prodi  : D4 - Teknik Informatika"
						};

						// Buat dan posisikan label-label informasi profil
						for (int i = 0; i < profileInfo.Length; i++)
						{
							profileLabels[i] = new Label();
							profileLabels[i].Text = profileInfo[i];

							// Posisikan label
							float yPos = iconTextLabel.Position.Y + iconTextLabel.Size.Y + 24 + (i * (24 + 4));
							profileLabels[i].Position = new Vector2(iconTextLabel.Position.X, yPos);
							profileLabels[i].Size = new Vector2(bottomRectangle.Size.X - iconTextLabel.Position.X - 36, 24);

							// Gunakan font dan warna yang sama
							if (iconFont != null)
							{
								profileLabels[i].AddThemeFontOverride("font", iconFont);
								profileLabels[i].AddThemeFontSizeOverride("font_size", 24);
							}
							profileLabels[i].AddThemeColorOverride("font_color", new Color("#4D4D4D"));

							// Tambahkan label ke persegi kedua
							bottomRectangle.AddChild(profileLabels[i]);
						}
					};
					break;

				case "Quest":
					iconTextLabel.Text = "Informasi Proyek";

					// Tunggu sebentar untuk memastikan label lama sudah dihapus
					GetTree().CreateTimer(0.05).Timeout += () =>
					{
						// Tambahkan informasi proyek di bawah text "Informasi Proyek"
						string[] projectInfo = {
							"Tema : Suku Minahasa",
							"Motif :",
							" - Kain Bentenan",
							" - Alat Musik Kolintang"
						};

						// Buat dan posisikan label-label informasi proyek
						for (int i = 0; i < projectInfo.Length; i++)
						{
							profileLabels[i] = new Label();
							profileLabels[i].Text = projectInfo[i];

							// Posisikan label
							float yPos = iconTextLabel.Position.Y + iconTextLabel.Size.Y + 24 + (i * (24 + 4));
							profileLabels[i].Position = new Vector2(iconTextLabel.Position.X, yPos);
							profileLabels[i].Size = new Vector2(bottomRectangle.Size.X - iconTextLabel.Position.X - 36, 24);

							// Gunakan font dan warna yang sama
							if (iconFont != null)
							{
								profileLabels[i].AddThemeFontOverride("font", iconFont);
								profileLabels[i].AddThemeFontSizeOverride("font_size", 24);
							}
							profileLabels[i].AddThemeColorOverride("font_color", new Color("#4D4D4D"));

							// Tambahkan label ke persegi kedua
							bottomRectangle.AddChild(profileLabels[i]);
						}
					};
					break;

				case "Location":
					iconTextLabel.Text = "Contact";

					// Tunggu sebentar untuk memastikan label lama sudah dihapus
					GetTree().CreateTimer(0.05).Timeout += () =>
					{
						// Tambahkan informasi kontak di bawah text "Contact"
						string[] contactInfo = {
							"Email         : harioobmo@gmail.com",
							"Github       : rioBMO",
							"Instagram : ryobmo_"
						};

						// Buat dan posisikan label-label informasi kontak
						for (int i = 0; i < contactInfo.Length; i++)
						{
							profileLabels[i] = new Label();
							profileLabels[i].Text = contactInfo[i];

							// Posisikan label
							float yPos = iconTextLabel.Position.Y + iconTextLabel.Size.Y + 24 + (i * (24 + 4));
							profileLabels[i].Position = new Vector2(iconTextLabel.Position.X, yPos);
							profileLabels[i].Size = new Vector2(bottomRectangle.Size.X - iconTextLabel.Position.X - 36, 24);

							// Gunakan font dan warna yang sama
							if (iconFont != null)
							{
								profileLabels[i].AddThemeFontOverride("font", iconFont);
								profileLabels[i].AddThemeFontSizeOverride("font_size", 24);
							}
							profileLabels[i].AddThemeColorOverride("font_color", new Color("#4D4D4D"));

							// Tambahkan label ke persegi kedua
							bottomRectangle.AddChild(profileLabels[i]);
						}
					};
					break;

				default:
					iconTextLabel.Text = "";
					break;
			}
		}
		else
		{
			GD.Print($"Tidak dapat memuat icon: {iconPath}");
		}
	}
}
