CREATE FUNCTION twitch.stream_insert (
    channel_ VARCHAR,
    start_date_ TIMESTAMP without TIME zone
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
        );

    RETURN found;
END;$$;

ALTER FUNCTION twitch.stream_insert (
    VARCHAR,
    TIMESTAMP
    ) OWNER TO markr;
