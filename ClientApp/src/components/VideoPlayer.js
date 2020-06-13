import React, {Component} from 'react';
import AzureMediaPlayer from "./AzureMediaPlayer";
import Loader from "./Loader";
import Card from "@material-ui/core/Card";

export class VideoPlayer extends Component {
    static displayName = VideoPlayer.name;
    constructor(props) {
        super(props);
        const {videoName, videoUrl} = props.location.state
        this.state = {
            videoId: null,
            videoData: {videoName, videoUrl},
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
            <Card>
                <AzureMediaPlayer sourceVideo={sourceVideo}/>
            </Card>
        );
    }

    static renderLoading() {
        return (
            <div>
                <Loader/>
                <p><em>Loading...</em></p>
            </div>
        )
    }

    componentDidMount = async () => {
        // We retrieve by the logical id of the artifact
        if (this.state.videoId !== null) {
            this.state.videoData = null;
            await fetch('/media/video/' + this.state.videoId).then(async response => await response.json())
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
    };

    render() {
        let contents = null;
        let title = 'Loading...';
        if (this.state.videoData !== null) {
            title = this.state.videoData?.videoName;
            contents = VideoPlayer.renderVideo(this.state.videoData)
        }
        if (this.state.videoData === null) {
            contents = VideoPlayer.renderLoading()
        }
        // Maybe not the best practice
        if (this.state.error === true) {
            throw new Error(this.state.errorMessage)
        }

        return (
            <div>
                <h1 id="tabelLabel">{title}</h1>
                {contents}
            </div>
        );
    }
}