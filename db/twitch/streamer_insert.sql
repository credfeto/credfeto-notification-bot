create function twitch.streamer_insert(username_ text, datecreated_ timestamp with time zone) returns boolean
    language plpgsql
as
$$
begin
    insert into twitch.streamer
    (
        username,
        datecreated
    )
    values
        (
            userName_,
            dateCreated_
        );

    return FOUND;
end
$$;

alter function twitch.streamer_insert(text, timestamp with time zone) owner to markr;

