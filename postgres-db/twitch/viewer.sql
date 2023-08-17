CREATE TABLE twitch.viewer (
    username VARCHAR(100) NOT NULL CONSTRAINT viewer_pk PRIMARY KEY,
    date_created TIMESTAMP NOT NULL,
    id VARCHAR(100) NOT NULL
    );

ALTER TABLE twitch.viewer OWNER TO markr;

GRANT DELETE,
    INSERT,
    SELECT,
    UPDATE
    ON twitch.viewer
    TO notificationbot;
