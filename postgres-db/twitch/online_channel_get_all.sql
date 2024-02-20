CREATE FUNCTION twitch.online_channel_get_all ()
RETURNS TABLE (
    username VARCHAR,
    date_created TIMESTAMP without TIME zone
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.username, s.date_created FROM twitch.online_channel s);
END;$$;

ALTER FUNCTION twitch.online_channel_get_all () OWNER TO markr;

GRANT EXECUTE
    ON FUNCTION twitch.online_channel_get_all()
    TO notificationbot;
