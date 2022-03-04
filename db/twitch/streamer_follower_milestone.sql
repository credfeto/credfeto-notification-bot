CREATE TABLE twitch.streamer_follower_milestone (
    channel_name VARCHAR(100) NOT NULL,
    followers_reached INT NOT NULL,
    when_reached TIMESTAMP NOT NULL,
    CONSTRAINT streamer_follower_milestone_pk PRIMARY KEY (
        channel_name,
        followers_reached
        )
    );
