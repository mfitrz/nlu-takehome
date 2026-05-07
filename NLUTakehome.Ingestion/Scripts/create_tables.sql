-- Violations --
-- Only has records that are more recent January 1st, 2024
CREATE TABLE violations (
    id                      INTEGER PRIMARY KEY,
    address                 TEXT NOT NULL,
    violation_date          DATE,
    violation_code          TEXT,
    violation_status        TEXT,
    violation_desc          TEXT,
    violation_inspc_comms   TEXT
);

CREATE INDEX idx_violations_address ON violations (address);
CREATE INDEX idx_violations_date ON violations (violation_date);

-- Scofflaws --
-- Stores addresses that have been flagged as scofflaws.
CREATE TABLE scofflaws (
    id                    SERIAL PRIMARY KEY,
    address               TEXT NOT NULL UNIQUE
);

CREATE INDEX idx_scofflaws_address on scofflaws (address);

-- Comments --
-- Stores user-submitted comments about specific addresses
CREATE TABLE comments (
    id                    SERIAL PRIMARY KEY,
    address               TEXT NOT NULL,
    author                TEXT NOT NULL,
    comment_text          TEXT NOT NULL,
    created_at            TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_comments_address ON comments (address);