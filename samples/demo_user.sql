-- Run after the application has created Users.db.
-- Execute in DB Browser for SQLite, then select Write Changes.
-- Coursework demo account: demo / Demo123!
-- An existing demo account is left unchanged.
INSERT INTO "Users" ("Name", "Password")
SELECT 'demo', 'Demo123!'
WHERE NOT EXISTS (
    SELECT 1 FROM "Users" WHERE "Name" = 'demo'
);
