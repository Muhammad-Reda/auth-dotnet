Tentu, ini adalah **Checklist Tugas (To-Do List)** yang disusun berdasarkan alur kerja Gitflow yang kita bahas. Anda bisa menyalin ini ke dalam file `README.md` atau aplikasi manajemen tugas (seperti Trello/Notion) untuk mencentangnya satu per satu.

### 🟢 Phase 0: Persiapan (Foundation)

**Branch:** `feature/init-setup`
**Tujuan:** Membuat kerangka project agar bisa dijalankan.

- [ ] **Inisialisasi Project:** Jalankan `npm init` (atau `go mod init`, dsb).
- [ ] **Setup Git:** `git init` dan buat file `.gitignore` (masukkan `node_modules`, `.env`, dll).
- [ ] **Environment Variables:** Buat file `.env` dan `.env.example` untuk menyimpan kredensial database & port.
- [ ] **Install Core Dependencies:** Install driver database, framework web (Express/Gin/Laravel), dan library config (`dotenv`).
- [ ] **Hello World:** Buat route sederhana `GET /` untuk memastikan server berjalan.
- [ ] **Commit & Merge:** Push ke `develop`.

---

### 🔵 Phase 1: Database Architecture

**Branch:** `feature/database-migration`
**Tujuan:** Menerapkan desain DB Users & Profile ke sistem.

- [ ] **Setup ORM/Query Builder:** Konfigurasi koneksi ke database (Sequelize/TypeORM/Prisma/Gorm).
- [ ] **Migration Table `Users`:**
    - [ ] Kolom: `UUID` (PK), `Email` (Unique), `Username`, `Password`, `Role`.
    - [ ] Timestamps: `created_at`, `updated_at`, `deleted_at`.
- [ ] **Migration Table `Profile`:**
    - [ ] Kolom: `user_id` (PK & FK ke Users), `Full Name`, `Address`, dll.
    - [ ] Relasi: Pastikan Foreign Key terpasang dengan benar (`ON DELETE CASCADE`).
- [ ] **Run Migration:** Jalankan perintah migrasi dan cek hasilnya di Database GUI (DBeaver/TablePlus).
- [ ] **Commit & Merge:** Push ke `develop`.

---

### 🟠 Phase 2: Registrasi (The Gate)

**Branch:** `feature/auth-register`
**Tujuan:** User bisa mendaftar dan data tersimpan di DUA tabel sekaligus.

- [ ] **Install Security Libs:** Install `bcrypt` (untuk hashing) dan validator input.
- [ ] **Endpoint `POST /auth/register`:**
    - [ ] Validasi Input: Pastikan email valid & password kuat.
    - [ ] Cek Duplikasi: Pastikan email/username belum terdaftar.
    - [ ] Hashing: Hash password user sebelum disimpan.
- [ ] **Database Transaction (Penting!):**
    - [ ] Bungkus query dalam _Transaction_.
    - [ ] Insert ke tabel `Users`.
    - [ ] Insert ke tabel `Profile` (dengan `user_id` dari user yang baru dibuat).
    - [ ] Commit transaction jika sukses, Rollback jika gagal.
- [ ] **Test Manual:** Coba register via Postman, pastikan data masuk ke kedua tabel.
- [ ] **Commit & Merge:** Push ke `develop`.

---

### 🟣 Phase 3: Login & Token

**Branch:** `feature/auth-login`
**Tujuan:** User bisa masuk dan mendapatkan "kunci akses" (Token).

- [ ] **Install JWT Lib:** Install `jsonwebtoken` (atau library session manager).
- [ ] **Endpoint `POST /auth/login`:**
    - [ ] Cari user berdasarkan email.
    - [ ] Bandingkan password input dengan hash di DB (`bcrypt.compare`).
    - [ ] Jika valid, generate JWT Token (masukkan `user_id` dan `role` dalam payload).
- [ ] **Middleware Auth:** Buat middleware untuk memverifikasi token pada route yang dilindungi.
- [ ] **Test Manual:** Login -> Dapat Token -> Akses route terproteksi menggunakan token.
- [ ] **Commit & Merge:** Push ke `develop`.

---

### 🟡 Phase 4: Manajemen Profil

**Branch:** `feature/user-profile`
**Tujuan:** User bisa melihat dan mengedit data diri mereka.

- [ ] **Endpoint `GET /me` (Lihat Profil Sendiri):**
    - [ ] Ambil `user_id` dari Token (via Middleware).
    - [ ] Query ke DB: Join tabel `Users` dan `Profile`.
    - [ ] Return data JSON (Hati-hati: Jangan kirim balik field `password`).
- [ ] **Endpoint `PUT /profile` (Update Data):**
    - [ ] Validasi input (misal: format no HP).
    - [ ] Update tabel `Profile` berdasarkan `user_id`.
    - [ ] Pastikan user tidak bisa mengedit profil orang lain.
- [ ] **Commit & Merge:** Push ke `develop`.

---

### 🔴 Phase 5: Final Polish & Release

**Branch:** `fix/pre-release`
**Tujuan:** Membersihkan kode sebelum "Go Live".

- [ ] **Error Handling:** Pastikan API mereturn error code yang rapi (400, 401, 404, 500) bukan error log mentah.
- [ ] **Code Cleanup:** Hapus `console.log` debug dan komentar yang tidak perlu.
- [ ] **Readme:** Update dokumentasi cara menjalankan project.
- [ ] **Merge to Main:** Gabungkan `develop` ke `main` dan beri Tag versi (v1.0.0).

---

### Tips Simulasi

Agar terasa seperti kerja tim beneran:

1. Setiap kali selesai satu poin checklist di atas, lakukan **Commit** dengan pesan jelas.
   Contoh: `git commit -m "feat: implement bcrypt hashing on register"`
2. **Jangan loncat branch!** Selesaikan satu fitur sampai tuntas (termasuk testing di Postman) baru pindah ke fitur berikutnya.

Siap untuk memulai Phase 0?
