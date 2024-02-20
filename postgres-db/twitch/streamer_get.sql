CREATE FUNCTION twitch.streamer_get (username_ VARCHAR)
RETURNS TABLE (
    username VARCHAR,
    id VARCHAR,
    date_created TIMESTAMP without TIME zone
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.username, s.id, s.date_created FROM twitch.streamer s WHERE s.username = username_);
END;$$;

ALTER FUNCTION twitch.streamer_get (VARCHAR) OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.streamer_get(VARCHAR)
    TO notificationbot;
