

CREATE DATABASE IF NOT EXISTS customer_management;
USE customer_management;

-- ============================================
-- BẢNG USER: đăng nhập, không phân quyền
-- ============================================
CREATE TABLE user (
    user_id         VARCHAR(36) PRIMARY KEY DEFAULT (UUID()),
    username        VARCHAR(50) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- BẢNG CUSTOMER: theo đúng UI danh sách + validate
-- ============================================
CREATE TABLE customer (
    customer_id             VARCHAR(36) PRIMARY KEY DEFAULT (UUID()),

    customer_code           VARCHAR(20) NOT NULL UNIQUE,     -- KHyyyyMMxxxxxx

    customer_type           VARCHAR(50),                     -- Loại khách hàng (NBH01, VIP, LKHA, ...)
    full_name               VARCHAR(128) NOT NULL,           -- Tên khách hàng

    tax_code                VARCHAR(50),                     -- Mã số thuế
    shipping_address        VARCHAR(255),                    -- Địa chỉ giao hàng
    billing_address         VARCHAR(255),                    -- Địa chỉ hóa đơn

    phone                   VARCHAR(11) UNIQUE,              -- 10-11 số
    email                   VARCHAR(100) UNIQUE,             -- định dạng email

    last_purchase_date      DATE,                            -- Ngày mua gần nhất
    purchased_summary       VARCHAR(255),                    -- Hàng hóa đã mua (tổng quát)
    last_purchased_item     VARCHAR(255),                    -- Tên hàng hóa đã mua (mới nhất)

    created_at              DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at              DATETIME DEFAULT CURRENT_TIMESTAMP 
                            ON UPDATE CURRENT_TIMESTAMP
);

-- INDEXES
CREATE INDEX ix_customer_full_name   ON customer(full_name);
CREATE INDEX ix_customer_phone       ON customer(phone);
CREATE INDEX ix_customer_email       ON customer(email);
CREATE INDEX ix_customer_type        ON customer(customer_type);

-- ============================================
-- HÀM SINH MÃ KHÁCH HÀNG KHyyyyMM000001
-- ============================================
DROP FUNCTION IF EXISTS fn_generate_customer_code;
DELIMITER $$

CREATE FUNCTION fn_generate_customer_code()
RETURNS VARCHAR(20)
DETERMINISTIC
BEGIN
    DECLARE prefix VARCHAR(10);
    DECLARE current_ym VARCHAR(6);
    DECLARE last_number INT;
    DECLARE new_number INT;
    DECLARE result VARCHAR(20);

    SET current_ym = DATE_FORMAT(NOW(), '%Y%m');
    SET prefix = CONCAT('KH', current_ym);

    SELECT CAST(SUBSTRING(customer_code, 11, 6) AS UNSIGNED)
    INTO last_number
    FROM customer
    WHERE customer_code LIKE CONCAT(prefix, '%')
    ORDER BY customer_code DESC
    LIMIT 1;

    IF last_number IS NULL THEN 
        SET new_number = 1;
    ELSE
        SET new_number = last_number + 1;
    END IF;

    SET result = CONCAT(prefix, LPAD(new_number, 6, '0'));
    RETURN result;
END$$

DELIMITER ;

-- ============================================
-- TRIGGER TỰ ĐỘNG SINH customer_code
-- ============================================
DROP TRIGGER IF EXISTS trg_customer_before_insert;
DELIMITER $$

CREATE TRIGGER trg_customer_before_insert
BEFORE INSERT ON customer
FOR EACH ROW
BEGIN
    IF NEW.customer_code IS NULL OR NEW.customer_code = '' THEN
        SET NEW.customer_code = fn_generate_customer_code();
    END IF;
END$$

DELIMITER ;


INSERT INTO customer (
    customer_type, full_name, tax_code, shipping_address, billing_address,
    phone, email, last_purchase_date, purchased_summary, last_purchased_item
)
VALUES
('VIP', 'Nguyễn Văn A', '0101245789',
 '123 Lê Lợi, Hà Nội',
 '123 Lê Lợi, Hà Nội',
 '0987654321', 'a@example.com',
 '2025-11-10',
 'Điện thoại, Laptop',
 'Laptop Dell XPS 13'),

('Thân thiết', 'Trần Thị B', '0201123456',
 '45 Trần Hưng Đạo, Đà Nẵng',
 '45 Trần Hưng Đạo, Đà Nẵng',
 '0911222333', 'b@example.com',
 '2025-11-11',
 'Quần áo, Mỹ phẩm',
 'Kem dưỡng da'),

('Mới', 'Hoàng Chí C', '0309988776',
 '88 Nguyễn Huệ, TP.HCM',
 '88 Nguyễn Huệ, TP.HCM',
 '0909090909', 'c@example.com',
 '2025-10-29',
 'Phụ kiện PC',
 'Bàn phím Akko 3098'),

('VIP', 'Lê Văn D', '0403344556',
 '12 Lạc Long Quân, TP.HCM',
 '12 Lạc Long Quân, TP.HCM',
 '0977888999', 'd@example.com',
 '2025-11-12',
 'Đồ gia dụng',
 'Nồi chiên không dầu');

INSERT INTO user (username, password_hash)
VALUES (
    'admin',
    '$2a$10$7QJ7G1x7dDYrVdzSsfXr/OdT95YtTpr0aS/z5tH6tFNEwG.ZVS2k6'
);
