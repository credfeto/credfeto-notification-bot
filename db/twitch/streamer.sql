CREATE TABLE twitch.streamer (
    username VARCHAR(100) NOT NULL CONSTRAINT streamer_pk PRIMARY KEY,
    date_created TIMESTAMP NOT NULL,
    id VARCHAR(100) NOT NULL
    );

ALTER TABLE twitch.streamer OWNER TO markr;
