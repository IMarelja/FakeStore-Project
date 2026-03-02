CREATE DATABASE fakestore
use fakestore

CREATE TABLE brand (
    brand_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE category (
    category_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE users (
    user_id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE product (
    product_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    unit VARCHAR(50),
    image VARCHAR(500),
    discount INT NOT NULL DEFAULT 0,
    available BOOLEAN NOT NULL DEFAULT TRUE,
    brand_id INT NOT NULL,
    category_id INT NOT NULL
);

CREATE TABLE review (
    review_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    rating INT NOT NULL,
    comment TEXT
);

CREATE TABLE cart (
    cart_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL
);

CREATE TABLE cart_item (
    cart_item_id INT PRIMARY KEY AUTO_INCREMENT,
    cart_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL
);

CREATE TABLE order_status (
    order_status_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE orders (
    order_id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    order_status_id INT NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL DEFAULT 0.00
);

CREATE TABLE order_item (
    order_item_id INT PRIMARY KEY AUTO_INCREMENT,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL
);

ALTER TABLE product
    ADD CONSTRAINT fk_product_brand
        FOREIGN KEY (brand_id) REFERENCES brand (brand_id),
    ADD CONSTRAINT fk_product_category
        FOREIGN KEY (category_id) REFERENCES category (category_id);

ALTER TABLE review
    ADD CONSTRAINT fk_review_user
        FOREIGN KEY (user_id) REFERENCES users (user_id),
    ADD CONSTRAINT fk_review_product
        FOREIGN KEY (product_id) REFERENCES product (product_id);

ALTER TABLE cart
    ADD CONSTRAINT fk_cart_user
        FOREIGN KEY (user_id) REFERENCES users (user_id);

ALTER TABLE cart_item
    ADD CONSTRAINT fk_cart_item_cart
        FOREIGN KEY (cart_id) REFERENCES cart (cart_id),
    ADD CONSTRAINT fk_cart_item_product
        FOREIGN KEY (product_id) REFERENCES product (product_id);

ALTER TABLE orders
    ADD CONSTRAINT fk_order_user
        FOREIGN KEY (user_id) REFERENCES users (user_id),
    ADD CONSTRAINT fk_order_status
        FOREIGN KEY (order_status_id) REFERENCES order_status (order_status_id);

ALTER TABLE order_item
    ADD CONSTRAINT fk_order_item_order
        FOREIGN KEY (order_id) REFERENCES orders (order_id),
    ADD CONSTRAINT fk_order_item_product
        FOREIGN KEY (product_id) REFERENCES product (product_id);

CREATE INDEX idx_product_brand_id ON product (brand_id);
CREATE INDEX idx_product_category_id ON product (category_id);
CREATE INDEX idx_review_user_id ON review (user_id);
CREATE INDEX idx_review_product_id ON review (product_id);
CREATE INDEX idx_cart_user_id ON cart (user_id);
CREATE INDEX idx_cart_item_cart_id ON cart_item (cart_id);
CREATE INDEX idx_cart_item_product_id ON cart_item (product_id);
CREATE INDEX idx_order_user_id ON orders (user_id);
CREATE INDEX idx_order_status_id ON orders (order_status_id);
CREATE INDEX idx_order_item_order_id ON order_item (order_id);
CREATE INDEX idx_order_item_product_id ON order_item (product_id);
