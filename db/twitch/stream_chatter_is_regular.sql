CREATE FUNCTION twitch.stream_chatter_is_regular (
    channel_ VARCHAR,
    username_ VARCHAR
    )
RETURNS TABLE (
    chat_user VARCHAR,
    regular boolean
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN query(SELECT username_ AS chat_user, EXISTS (
                SELECT chat_user
                FROM twitch.stream_chatter
                WHERE channel = channel_
                    AND chat_user = username_
                GROUP BY chat_user
                HAVING count(*) > 2
                ) AS regular);
END;$$;

ALTER FUNCTION twitch.stream_chatter_is_regular (
    VARCHAR,
    VARCHAR
    ) OWNER TO markr;
