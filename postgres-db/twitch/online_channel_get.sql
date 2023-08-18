CREATE FUNCTION twitch.online_channel_get(username_ VARCHAR)
    RETURNS TABLE
            (
                username     VARCHAR,
                date_created TIMESTAMP without TIME zone
            )
    LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY (SELECT s.username, s.date_created FROM twitch.online_channel s WHERE s.username = username_);
END;
$$;

ALTER FUNCTION twitch.online_channel_get (VARCHAR) OWNER TO markr;

GRANT EXECUTE ON FUNCTION twitch.online_channel_get (VARCHAR) TO notificationbot;
