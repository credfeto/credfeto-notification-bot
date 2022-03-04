CREATE PROCEDURE twitch.stream_insert (
    channel_ VARCHAR,
    start_date_ TIMESTAMP without TIME zone
    ) LANGUAGE sql
AS
$$

INSERT INTO twitch.stream (
    channel,
    start_date
    )
VALUES (
    channel_,
    start_date_
    );$$;

ALTER PROCEDURE twitch.stream_insert (
    VARCHAR,
    TIMESTAMP
    ) OWNER TO markr;
