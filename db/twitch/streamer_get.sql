create function twitch.streamer_get(username_ character varying)
    returns TABLE(username character varying, id character varying, date_created timestamp without time zone)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY(SELECT s.username, s.id, s.date_created FROM twitch.streamer s WHERE s.username = username_);
END;
$$;

alter function twitch.streamer_get(varchar) owner to markr;

