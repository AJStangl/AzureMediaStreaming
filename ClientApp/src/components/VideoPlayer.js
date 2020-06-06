﻿import React, {Component} from 'react';
import Player from "./Player";

export class VideoPlayer extends Component {
    static displayName = VideoPlayer.name;

    constructor(props) {
        super(props);
        this.state = {videoData: {}, loading: true};
    }

    componentDidMount() {
        this.getVideoData();
    }

    static renderVideo(videoData) {
        var sourceVideo = {
            "src": videoData.videoUrl,
            "type": "application/vnd.ms-sstr+xml"
        };
        console.log(sourceVideo);
        return (
            <Player sourceVideo={sourceVideo} />
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : VideoPlayer.renderVideo(this.state.videoData);

        return (
            <div>
                <h1 id="tabelLabel">Video Demo</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }

    async getVideoData() {
        const response = await fetch('/media/video');
        const data = await response.json();
        console.log(data)
        this.setState({videoData: data, loading: false});
    }
}