-- Runs automatically on first `docker compose up` via /docker-entrypoint-initdb.d/
-- The `fakestore` database is already created by POSTGRES_DB env var, so we skip CREATE DATABASE.

-- ─────────────────────────────────────────────
-- Tables
-- ─────────────────────────────────────────────

CREATE TABLE users (
    user_id  SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email    VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role     VARCHAR(100) NOT NULL DEFAULT 'read-only'
);

CREATE TABLE product (
    product_id  SERIAL PRIMARY KEY,
    name        VARCHAR(255)     NOT NULL,
    description TEXT,
    price       DECIMAL(10, 2)   NOT NULL,
    unit        VARCHAR(50),
    image       VARCHAR(500),
    discount    INT              NOT NULL DEFAULT 0,
    available   BOOLEAN          NOT NULL DEFAULT TRUE,
    brand       VARCHAR(100)     NOT NULL,
    category    VARCHAR(100)     NOT NULL,
    rating      DOUBLE PRECISION NOT NULL DEFAULT 0
);

CREATE TABLE review (
    review_id  SERIAL PRIMARY KEY,
    user_id    INT NOT NULL REFERENCES users (user_id),
    product_id INT NOT NULL REFERENCES product (product_id),
    rating     INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment    TEXT
);

CREATE TABLE cart (
    cart_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL UNIQUE REFERENCES users (user_id)
);

CREATE TABLE cart_item (
    cart_item_id SERIAL PRIMARY KEY,
    cart_id      INT NOT NULL REFERENCES cart (cart_id),
    product_id   INT NOT NULL REFERENCES product (product_id),
    quantity     INT NOT NULL CHECK (quantity > 0),
    UNIQUE (cart_id, product_id)
);

CREATE TABLE orders (
    order_id     SERIAL PRIMARY KEY,
    user_id      INT            NOT NULL REFERENCES users (user_id),
    order_status VARCHAR(100)   NOT NULL DEFAULT 1,
    total_price  DECIMAL(10, 2) NOT NULL DEFAULT 0.00
);

CREATE TABLE order_item (
    order_item_id SERIAL PRIMARY KEY,
    order_id      INT NOT NULL REFERENCES orders (order_id),
    product_id    INT NOT NULL REFERENCES product (product_id),
    quantity      INT NOT NULL CHECK (quantity > 0)
);

-- ─────────────────────────────────────────────
-- Indexes
-- ─────────────────────────────────────────────

CREATE INDEX idx_review_user_id        ON review (user_id);
CREATE INDEX idx_review_product_id     ON review (product_id);
CREATE INDEX idx_cart_item_cart_id     ON cart_item (cart_id);
CREATE INDEX idx_cart_item_product_id  ON cart_item (product_id);
CREATE INDEX idx_order_user_id         ON orders (user_id);
CREATE INDEX idx_order_item_order_id   ON order_item (order_id);
CREATE INDEX idx_order_item_product_id ON order_item (product_id);
