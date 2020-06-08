import React, {Component} from 'react';
import AzureMediaPlayer from "./AzureMediaPlayer";

export class VideoPlayer extends Component {
    static displayName = VideoPlayer.name;

    constructor(props) {
        super(props);
        this.state = {
            videoData: null,
            loading: true,
            error: false,
            errorMessage: ''
        };
    }

    static renderVideo(videoData) {
        let sourceVideo = {
            "src": videoData?.videoUrl,
            "type": "application/vnd.ms-sstr+xml"
        };
        return (
            <AzureMediaPlayer sourceVideo={sourceVideo}/>
        );
    }

    static renderLoading() {
        return (
            <p><em>Loading...</em></p>
        )
    }

    async componentDidMount() {
        await fetch('/media/video').then(async response => await response.json())
            .then(data => {
                this.setState({
                    videoData: data,
                    loading: false
                })
            })
            .catch(err => this.setState({
                videoData: {},
                loading: false,
                error: true,
                errorMessage: err
            }));
    }

    render() {
        let contents = null;
        let title = 'Loading...';
        if (this.state.videoData === null) {
            contents = VideoPlayer.renderLoading()
        }
        if (this.state.error === true) {
            throw new Error(this.state.errorMessage)
        }
        if (this.state.loading === true) {
            contents = VideoPlayer.renderLoading()
        } else {
            console.log("Video is rendering...")
            title = this.state.videoData?.videoName;
            contents = VideoPlayer.renderVideo(this.state.videoData)
        }

        return (
            <div>
                <h1 id="tabelLabel">{title}</h1>
                {contents}
            </div>
        );
    }
}