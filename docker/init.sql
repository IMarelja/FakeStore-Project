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
    role     VARCHAR(100) NOT NULL
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
    order_status INT            NOT NULL DEFAULT 1,
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

-- ─────────────────────────────────────────────
-- Seed Data
-- ─────────────────────────────────────────────

WITH
  ins_users AS (
    INSERT INTO users (username, email, password, role)
    VALUES
        ('alice',   'alice@example.com',   'hashed_password_1', 'read-only'),
        ('bob',     'bob@example.com',     'hashed_password_2', 'read-only'),
        ('charlie', 'charlie@example.com', 'hashed_password_3', 'read-only'),
        ('admin',   'admin@example.com',   'admin',             'full access')
    RETURNING user_id, username
  ),
  ins_products AS (
    INSERT INTO product (name, description, price, unit, image, discount, available, brand, rating)
    VALUES
        ('iPhone 15',         'Latest Apple smartphone',            999.99, 'piece', 'iphone15.jpg',      0,  TRUE, 'Apple',   4.5),
        ('Galaxy S24',        'Samsung flagship smartphone',        849.99, 'piece', 'galaxys24.jpg',     10, TRUE, 'Samsung', 4.3),
        ('Sony WH-1000XM5',   'Noise-cancelling headphones',        349.99, 'piece', 'sonywh1000xm5.jpg',  5, TRUE, 'Sony',    4.8),
        ('Nike Air Max 90',   'Classic running shoes',              120.00, 'pair',  'airmax90.jpg',       0, TRUE, 'Nike',    4.2),
        ('Adidas Ultraboost', 'High performance running shoes',     180.00, 'pair',  'ultraboost.jpg',    15, TRUE, 'Adidas',  4.6),
        ('Apple Watch S9',    'Smartwatch with health tracking',    399.99, 'piece', 'applewatch9.jpg',    0, TRUE, 'Apple',   4.4),
        ('Samsung 4K TV',     '55-inch 4K QLED television',        799.99, 'piece', 'samsungtv.jpg',     20, TRUE, 'Samsung', 4.1),
        ('Nike Dri-FIT Tee',  'Moisture-wicking training t-shirt',  35.00, 'piece', 'drifit.jpg',         0, TRUE, 'Nike',    4.0)
    RETURNING product_id, name
  ),
  ins_carts AS (
    INSERT INTO cart (user_id)
    SELECT user_id FROM ins_users WHERE username IN ('alice', 'bob', 'charlie')
    RETURNING cart_id, user_id
  ),
  ins_orders AS (
    INSERT INTO orders (user_id, order_status, total_price)
    SELECT u.user_id, v.order_status, v.total_price
    FROM ins_users u
    JOIN (VALUES
        ('alice',   4, 1349.98::DECIMAL(10,2)),
        ('bob',     3,  240.00::DECIMAL(10,2)),
        ('charlie', 2,  180.00::DECIMAL(10,2))
    ) AS v(username, order_status, total_price) ON u.username = v.username
    RETURNING order_id, user_id
  ),
  ins_reviews AS (
    INSERT INTO review (user_id, product_id, rating, comment)
    SELECT u.user_id, p.product_id, v.rating, v.comment
    FROM (VALUES
        ('alice',   'iPhone 15',         5, 'Amazing phone, best camera I have used.'),
        ('bob',     'iPhone 15',         4, 'Great phone but a bit pricey.'),
        ('charlie', 'Galaxy S24',        5, 'Excellent display and battery life.'),
        ('alice',   'Sony WH-1000XM5',   5, 'Best headphones on the market.'),
        ('bob',     'Nike Air Max 90',   4, 'Very comfortable and stylish.'),
        ('charlie', 'Adidas Ultraboost', 5, 'Best running shoes I have ever owned.'),
        ('alice',   'Samsung 4K TV',     4, 'Incredible picture quality.')
    ) AS v(username, product_name, rating, comment)
    JOIN ins_users u    ON u.username = v.username
    JOIN ins_products p ON p.name     = v.product_name
    RETURNING review_id
  ),
  ins_cart_items AS (
    INSERT INTO cart_item (cart_id, product_id, quantity)
    SELECT c.cart_id, p.product_id, v.quantity
    FROM (VALUES
        ('alice',   'iPhone 15',         1),
        ('alice',   'Sony WH-1000XM5',   1),
        ('bob',     'Nike Air Max 90',   2),
        ('charlie', 'Adidas Ultraboost', 1),
        ('charlie', 'Nike Dri-FIT Tee',  3)
    ) AS v(username, product_name, quantity)
    JOIN ins_users u    ON u.username  = v.username
    JOIN ins_carts c    ON c.user_id   = u.user_id
    JOIN ins_products p ON p.name      = v.product_name
    RETURNING cart_item_id
  ),
  ins_order_items AS (
    INSERT INTO order_item (order_id, product_id, quantity)
    SELECT o.order_id, p.product_id, v.quantity
    FROM (VALUES
        ('alice',   'iPhone 15',         1),
        ('alice',   'Sony WH-1000XM5',   1),
        ('bob',     'Nike Air Max 90',   2),
        ('charlie', 'Adidas Ultraboost', 1)
    ) AS v(username, product_name, quantity)
    JOIN ins_users u    ON u.username = v.username
    JOIN ins_orders o   ON o.user_id  = u.user_id
    JOIN ins_products p ON p.name     = v.product_name
    RETURNING order_item_id
  )
SELECT 'Seed data inserted successfully';
