create procedure twitch.streamer_insert(username_ varchar(100), started_streaming_ TIMESTAMP)
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



create procedure twitch.stream_insert(channel_ varchar(100), start_date_ TIMESTAMP)
  language plpgsql
as
$$
begin

  insert into twitch.stream
  (
    channel,
    start_date
  )
  values
    (
      channel_,
      start_date
    );

end
$$;


create procedure twitch.streamer_insert(username_ varchar(100), started_streaming_ TIMESTAMP)
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



create procedure twitch.stream_chatter_insert(channel_ varchar(100), start_date_ TIMESTAMP, chat_user varchar(100))
  language plpgsql
as
$$
begin

  first_message_date_ TIMESTAMP := CURRENT_TIMESTAMP();

  insert into twitch.stream_chatter
  (
    channel,
    start_date,
    chat_user,
    first_message_date
  )
  values
    (
      channel_,
      start_date,
      chat_user_,
      first_message_date_
    );

end
$$;


