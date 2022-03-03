create procedure twitch.streamer_insert(username_ character varying, started_streaming_ date)
    language plpgsql
as
$$
begin

    insert into twitch.streamer
    (
        username,
        started_streaming
    )
    values
    (
        username_,
        started_streaming
    );

end
$$;


