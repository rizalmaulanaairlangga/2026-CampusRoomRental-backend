Diagram ASCII
+------------------+
|      rooms       |
+------------------+
| id (PK)          |
| name             |
| capacity         |
| status           |
| created_at       |
| updated_at       |
| deleted_at       |
+------------------+
         |
         | 1
         |
         | N
+------------------+
|     bookings     |
+------------------+
| id (PK)          |
| room_id (FK)     |
| start_time       |
| end_time         |
| status           |
| created_at       |
| updated_at       |
| deleted_at       |
+------------------+

Relasi Antar Tabel
- Room 1 — N Booking
- Satu ruangan bisa memiliki banyak booking
- Satu booking hanya milik satu room

Relasi:
rooms.id → bookings.room_id

Untuk MVP:
❌ Tidak pakai User dulu
❌ Tidak pakai approval workflow
❌ Tidak pakai audit log
✅ Fokus: Room bisa dibooking
✅ Validasi konflik dilakukan di backend logic (bukan DB constraint kompleks)