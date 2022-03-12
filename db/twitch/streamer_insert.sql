create function twitch.streamer_insert(username_ text, id_ text, date_created_ timestamp with time zone) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.streamer
    (
        username,
        id,
        date_created
    )
    values
        (
            userName_,
            id_,
            date_created_
        )
    on conflict do nothing;

    return FOUND;
end
$$;

alter function twitch.streamer_insert(text, text, timestamp with time zone) owner to markr;

