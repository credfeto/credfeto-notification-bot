create function twitch.streamer_get(username_ character varying)
    returns TABLE(username character varying, date_created timestamp without time zone)
    language plpgsql
as
$$
begin
    return QUERY(
        select
            s.username,
            s.date_created
        from twitch.streamer s
        where s.username = username_
    );
end;
$$;

alter function twitch.streamer_get(varchar) owner to markr;

