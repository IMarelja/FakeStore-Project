-- Runs automatically on first `docker compose up` via /docker-entrypoint-initdb.d/
-- The `fakestore` database is already created by POSTGRES_DB env var, so we skip CREATE DATABASE.

-- ─────────────────────────────────────────────
-- Tables
-- ─────────────────────────────────────────────

CREATE TABLE users (
    user_id  SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email    VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
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

INSERT INTO users (username, email, password) VALUES
    ('alice',   'alice@example.com',   'hashed_password_1'),
    ('bob',     'bob@example.com',     'hashed_password_2'),
    ('charlie', 'charlie@example.com', 'hashed_password_3');

INSERT INTO product (name, description, price, unit, image, discount, available, brand, rating) VALUES
    ('iPhone 15',         'Latest Apple smartphone',             999.99, 'piece', 'iphone15.jpg',       0, TRUE, 'Apple',   4.5),
    ('Galaxy S24',        'Samsung flagship smartphone',         849.99, 'piece', 'galaxys24.jpg',      10, TRUE, 'Samsung', 4.3),
    ('Sony WH-1000XM5',   'Noise-cancelling headphones',         349.99, 'piece', 'sonywh1000xm5.jpg',   5, TRUE, 'Sony',    4.8),
    ('Nike Air Max 90',   'Classic running shoes',               120.00, 'pair',  'airmax90.jpg',         0, TRUE, 'Nike',    4.2),
    ('Adidas Ultraboost', 'High performance running shoes',      180.00, 'pair',  'ultraboost.jpg',      15, TRUE, 'Adidas',  4.6),
    ('Apple Watch S9',    'Smartwatch with health tracking',     399.99, 'piece', 'applewatch9.jpg',      0, TRUE, 'Apple',   4.4),
    ('Samsung 4K TV',     '55-inch 4K QLED television',         799.99, 'piece', 'samsungtv.jpg',       20, TRUE, 'Samsung', 4.1),
    ('Nike Dri-FIT Tee',  'Moisture-wicking training t-shirt',   35.00, 'piece', 'drifit.jpg',            0, TRUE, 'Nike',    4.0);

INSERT INTO review (user_id, product_id, rating, comment) VALUES
    (1, 1, 5, 'Amazing phone, best camera I have used.'),
    (2, 1, 4, 'Great phone but a bit pricey.'),
    (3, 2, 5, 'Excellent display and battery life.'),
    (1, 3, 5, 'Best headphones on the market.'),
    (2, 4, 4, 'Very comfortable and stylish.'),
    (3, 5, 5, 'Best running shoes I have ever owned.'),
    (1, 7, 4, 'Incredible picture quality.');

INSERT INTO cart (user_id) VALUES (1), (2), (3);

INSERT INTO cart_item (cart_id, product_id, quantity) VALUES
    (1, 1, 1),
    (1, 3, 1),
    (2, 4, 2),
    (3, 5, 1),
    (3, 8, 3);

INSERT INTO orders (user_id, order_status, total_price) VALUES
    (1, 4, 1349.98),
    (2, 3,  240.00),
    (3, 2,  180.00);

INSERT INTO order_item (order_id, product_id, quantity) VALUES
    (1, 1, 1),
    (1, 3, 1),
    (2, 4, 2),
    (3, 5, 1);
