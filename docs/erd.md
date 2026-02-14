+------------------+
|      users       |
+------------------+
| id (PK)          |
| email            |
| role             |
| created_at       |
+------------------+
         |
         | 1
         |
         | N
+------------------+
|     bookings     |
+------------------+
| id (PK)          |
| user_id (FK)     |
| room_id (FK)     |
| start_time       |
| end_time         |
| status           |
| created_at       |
| updated_at       |
| deleted_at       |
+------------------+
         |
         | N
         |
         | 1
+------------------+
|      rooms       |
+------------------+
