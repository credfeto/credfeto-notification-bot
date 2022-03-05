CREATE FUNCTION twitch.stream_chatter_insert (
    channel_ VARCHAR,
    start_date_ TIMESTAMP without TIME zone,
    chat_user_ VARCHAR
    )
RETURNS boolean LANGUAGE plpgsql
AS
$$

BEGIN
    INSERT INTO twitch.stream_chatter (
        channel,
        start_date,
        chat_user,
        first_message_date
        )
    VALUES (
        channel_,
        start_date_,
        chat_user_,
        now()
        );

    RETURN found;
END;$$;

ALTER FUNCTION twitch.stream_chatter_insert (
    VARCHAR,
    TIMESTAMP,
    VARCHAR
    ) OWNER TO markr;
