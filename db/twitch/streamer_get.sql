CREATE FUNCTION twitch.streamer_get (username_ TEXT)
RETURNS TABLE (
    username VARCHAR,
    datecreated TIMESTAMP without TIME zone
    ) LANGUAGE plpgsql
AS
$$

BEGIN
    RETURN QUERY(SELECT s.username, s.datecreated FROM twitch.streamer s WHERE s.username = username_);
END;$$;

ALTER FUNCTION twitch.streamer_get (TEXT) OWNER TO markr;
