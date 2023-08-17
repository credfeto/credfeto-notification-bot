CREATE FUNCTION twitch.stream_insert (
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.stream (
        channel,
        start_date
        )
    VALUES (
        channel_,
        start_date_
        )
        ON conflict do nothing;

    RETURN found;
END;$$;

ALTER FUNCTION twitch.stream_insert (
    VARCHAR,
    TIMESTAMP WITH TIME zone
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_insert(VARCHAR, TIMESTAMP WITH TIME zone)
    TO notificationbot;
