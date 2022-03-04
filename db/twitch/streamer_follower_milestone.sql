create table twitch.streamer_follower_milestone
(
    channel_name      varchar(100) not null,
    followers_reached int          not null,
    when_reached      TIMESTAMP not null,
    constraint streamer_follower_milestone_pk
        primary key (channel_name, followers_reached)
);

