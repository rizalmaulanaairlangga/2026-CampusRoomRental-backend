Entity 1: Room
Fungsi

Master data ruangan yang bisa dibooking.
| Field      | Type       | Required | Keterangan            |
| ---------- | ---------- | -------- | --------------------- |
| id         | UUID / int | ✔        | Primary Key           |
| name       | string     | ✔        | Nama ruangan          |
| capacity   | int        | ✖        | Kapasitas             |
| status     | string     | ✔        | `active` / `inactive` |
| created_at | datetime   | ✔        | Waktu dibuat          |
| updated_at | datetime   | ✔        | Waktu update          |
| deleted_at | datetime   | ✖        | Soft delete           |

Entity 2: Booking
Fungsi

Menyimpan data peminjaman ruangan.
| Field      | Type       | Required | Keterangan             |
| ---------- | ---------- | -------- | ---------------------- |
| id         | UUID / int | ✔        | Primary Key            |
| room_id    | FK         | ✔        | Relasi ke Room         |
| start_time | datetime   | ✔        | Waktu mulai            |
| end_time   | datetime   | ✔        | Waktu selesai          |
| status     | string     | ✔        | `booked` / `cancelled` |
| created_at | datetime   | ✔        | Waktu dibuat           |
| updated_at | datetime   | ✔        | Waktu update           |
| deleted_at | datetime   | ✖        | Soft delete            |

