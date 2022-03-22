CREATE FUNCTION stream_chatter_is_regular (
    channel_ TEXT,
    username_ TEXT
    )
RETURNS TABLE (
    chat_user VARCHAR,
    regular boolean
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT c.chat_user, c.regular FROM (
            SELECT sc.chat_user,
                true AS regular
            FROM twitch.stream_chatter sc
            WHERE sc.channel = channel_
                AND sc.chat_user = username_
            GROUP BY sc.chat_user
            HAVING count(*) > 2
            
            UNION
            
            SELECT username_ AS chat_user,
                false AS regular
            ) AS c ORDER BY c.regular DESC limit 1);
END;$$;

ALTER FUNCTION stream_chatter_is_regular (
    TEXT,
    TEXT
    ) OWNER TO markr;
