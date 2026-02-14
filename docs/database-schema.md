Entity 1: Room
Fungsi

Master data ruangan yang bisa dibooking.
| Field      | Type             | Required   | Keterangan            |
| ---------- | ----------       | --------   | --------------------- |
| id         | UUID / int       | ✔         | Primary Key           |
| name       | string           | ✔         | Nama ruangan          |
| capacity   | int              | ✖         | Kapasitas             |
| status     | string           | ✔         | `active` / `inactive` |
| created_at | DateTimeOffset    | ✔        | Waktu dibuat          |
| updated_at | DateTimeOffset    | ✔        | Waktu update          |
| deleted_at | DateTimeOffset    | ✖        | Soft delete           |

Entity 2: Booking
Fungsi

Menyimpan data peminjaman ruangan.
| Field      | Type              | Required  | Keterangan             |
| ---------- | ----------        | --------  | ---------------------- |
| id         | UUID / int        | ✔        | Primary Key            |
| user_id    | FK (UUID)         | ✔        | Pemilik booking        |
| room_id    | FK                | ✔        | Relasi ke Room         |
| start_time | DateTimeOffset    | ✔        | Waktu mulai            |
| end_time   | DateTimeOffset    | ✔        | Waktu selesai          |
| status     | string            | ✔        | `booked` / `cancelled` / `approved` / `rejected` |
| created_at | DateTimeOffset    | ✔        | Waktu dibuat           |
| updated_at | DateTimeOffset    | ✔        | Waktu update           |
| deleted_at | DateTimeOffset    | ✖        | Soft delete            |

