create function twitch.streamer_get(username_ text)
    returns TABLE(username text, datecreated timestamp without time zone)
    language plpgsql
as
$$
begin
    return QUERY(
        select
            UserName,
            DateCreated
        from twitch.streamer
        where UserName = userName    
    );
end;
$$;

alter function twitch.streamer_get(text) owner to markr;

