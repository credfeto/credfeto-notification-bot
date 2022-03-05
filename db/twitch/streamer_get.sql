create function twitch.streamer_get(username_ text)
    returns TABLE(username text, datecreated timestamp without time zone)
    language plpgsql
as
$$
begin
    return QUERY(
        select
            s.username,
            s.datecreated
        from twitch.streamer s
        where s.username = username_
    );
end;
$$;

alter function twitch.streamer_get(text) owner to markr;

