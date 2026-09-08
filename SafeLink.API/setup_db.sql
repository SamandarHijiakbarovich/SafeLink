-- SafeLink PostgreSQL setup
-- psql -U postgres -f setup_db.sql

CREATE DATABASE safelink_db;
\c safelink_db;

-- EF Core migration avtomatik jadval yaratadi,
-- lekin manual nazorat uchun shu faylni saqlaymiz.

-- Tekshirish:
-- \dt   => jadvallar ro'yxati
-- \du   => foydalanuvchilar
