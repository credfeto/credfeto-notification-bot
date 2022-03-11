create function twitch.streamer_insert(username_ text, date_created_ timestamp with time zone) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.streamer
    (
        username,
        date_created
    )
    values
        (
            userName_,
            date_created_
        )
    on conflict do nothing;

    return FOUND;
end
$$;

alter function twitch.streamer_insert(text, timestamp with time zone) owner to markr;

