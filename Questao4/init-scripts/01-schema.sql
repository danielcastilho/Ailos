-- PostgreSQL
CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TABLE atendimentos (
    id UUID DEFAULT gen_random_uuid() NOT NULL,
    assunto VARCHAR(100) NOT NULL,
    ano INTEGER
);