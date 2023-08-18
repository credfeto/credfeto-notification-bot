CREATE FUNCTION twitch.viewer_get (username_ VARCHAR)
RETURNS TABLE (
    username VARCHAR,
    id VARCHAR,
    date_created TIMESTAMP without TIME zone
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.username, s.id, s.date_created FROM twitch.viewer s WHERE s.username = username_);
END;$$;

ALTER FUNCTION twitch.viewer_get (VARCHAR) OWNER TO markr;

GRANT EXECUTE ON FUNCTION twitch.viewer_get (VARCHAR) TO notificationbot;