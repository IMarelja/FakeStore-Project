WITH
  ins_users AS (
    INSERT INTO users (username, email, password, role)
    VALUES
        ('alice',   'alice@example.com',   'password1', 'read-only'),
        ('bobby',     'bobby@example.com',     'password2', 'read-only'),
        ('charlie', 'charlie@example.com', 'password3', 'read-only'),
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
    SELECT user_id FROM ins_users WHERE username IN ('alice', 'bobby', 'charlie')
    RETURNING cart_id, user_id
  ),
  ins_orders AS (
    INSERT INTO orders (user_id, order_status, total_price)
    SELECT u.user_id, v.order_status, v.total_price
    FROM ins_users u
    JOIN (VALUES
        ('alice',   'Shipped', 1349.98::DECIMAL(10,2)),
        ('bobby',     'Delivered',  240.00::DECIMAL(10,2)),
        ('charlie', 'Processing',  180.00::DECIMAL(10,2))
    ) AS v(username, order_status, total_price) ON u.username = v.username
    RETURNING order_id, user_id
  ),
  ins_reviews AS (
    INSERT INTO review (user_id, product_id, rating, comment)
    SELECT u.user_id, p.product_id, v.rating, v.comment
    FROM (VALUES
        ('alice',   'iPhone 15',         5, 'Amazing phone, best camera I have used.'),
        ('bobby',     'iPhone 15',         4, 'Great phone but a bit pricey.'),
        ('charlie', 'Galaxy S24',        5, 'Excellent display and battery life.'),
        ('alice',   'Sony WH-1000XM5',   5, 'Best headphones on the market.'),
        ('bobby',     'Nike Air Max 90',   4, 'Very comfortable and stylish.'),
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
        ('bobby',     'Nike Air Max 90',   2),
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
        ('bobby',     'Nike Air Max 90',   2),
        ('charlie', 'Adidas Ultraboost', 1)
    ) AS v(username, product_name, quantity)
    JOIN ins_users u    ON u.username = v.username
    JOIN ins_orders o   ON o.user_id  = u.user_id
    JOIN ins_products p ON p.name     = v.product_name
    RETURNING order_item_id
  )
SELECT 'Seed data inserted successfully';