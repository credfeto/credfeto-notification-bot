CREATE FUNCTION twitch.stream_chatter_get (
    channel_ VARCHAR,
    start_date_ TIMESTAMP WITH TIME zone,
    chat_user_ VARCHAR
    )
RETURNS TABLE (
    channel VARCHAR,
    start_date TIMESTAMP without TIME zone,
    chat_user VARCHAR,
    first_message_date TIMESTAMP without TIME zone
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.channel, s.start_date, s.chat_user, s.first_message_date FROM twitch.stream_chatter s WHERE s.channel = channel_
            AND s.start_date = start_date_
            AND s.chat_user = chat_user_);
END;$$;

ALTER FUNCTION twitch.stream_chatter_get (
    VARCHAR,
    TIMESTAMP WITH TIME zone,
    VARCHAR
    ) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.stream_chatter_get(VARCHAR, TIMESTAMP WITH TIME zone, VARCHAR)
    TO notificationbot;
