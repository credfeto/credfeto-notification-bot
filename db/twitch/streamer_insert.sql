create procedure twitch.streamer_insert(username_ varchar(100), started_streaming_ TIMESTAMP)
    language sql
as $$
  insert into twitch.streamer
  (
    username,
    started_streaming
  )
  values
  (
    username_,
    started_streaming_
  );
$$;

