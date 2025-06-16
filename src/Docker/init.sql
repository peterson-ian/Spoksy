CREATE TABLE users (
    id UUID PRIMARY KEY,
    identity_provider_id VARCHAR(200) NOT NULL UNIQUE, 
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    birth_date TIMESTAMPTZ NOT NULL,
    current_country VARCHAR(4) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_active_at TIMESTAMPTZ NOT NULL, 
    status VARCHAR(20) NOT NULL
);

CREATE TABLE user_languages (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    language VARCHAR(10) NOT NULL,
    proficiency_level VARCHAR(30) NOT NULL,
    started_learning_on TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT
);

CREATE TABLE chats (
    id UUID PRIMARY KEY,
    primary_language VARCHAR(10) NOT NULL,
    secondary_language VARCHAR(10) NOT NULL,
    status VARCHAR(20) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_active_at TIMESTAMPTZ NOT NULL
);

CREATE TABLE chat_participants (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    chat_id UUID NOT NULL,
    join_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    leave_at TIMESTAMPTZ,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat FOREIGN KEY (chat_id) REFERENCES chats(id) ON DELETE RESTRICT
);


CREATE TABLE messages (
    id UUID PRIMARY KEY,
    sender_id UUID NOT NULL,
    chat_id UUID NOT NULL,
    content TEXT,
    sent_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    edit_at TIMESTAMPTZ,
    delete_at TIMESTAMPTZ,
    is_read BOOLEAN DEFAULT FALSE, 
    is_edit BOOLEAN DEFAULT FALSE,
    is_delete BOOLEAN DEFAULT FALSE,
    CONSTRAINT fk_user FOREIGN KEY (sender_id) REFERENCES users(id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat FOREIGN KEY (chat_id) REFERENCES chats(id) ON DELETE RESTRICT
);