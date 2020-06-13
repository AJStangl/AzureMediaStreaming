import React, {Component} from 'react';
import Loader from "./Loader";
import {Link} from 'react-router-dom'

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            videoResponse: [
                {
                    id: '',
                    videoName: '',
                    videoUrl: '',
                    street: '',
                    zipCode: '',
                    city: '',
                    state: '',
                    createdDate: '',
                    date: '',
                    time: ''
                }],
            error: false,
            errorMessage: '',
            loading: true
        };
    }

    componentDidMount = async () => {
        await fetch('/media/LatestVideo')
            .then(async response => await response.json())
            .then(data => {
                this.setState({
                    videoResponse: data,
                    loading: false
                })
            })
            .catch(err => this.setState({
                videoData: {},
                loading: false,
                error: true,
                errorMessage: err
            }));
    };


    // TODO: Replace with a materialUI table for reactive filtering.
    renderTableData() {
        const videos = this.state.videoResponse
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Link</th>
                    <th>Date Recorded</th>
                    <th>Time Recorded</th>
                    <th>Address</th>
                    <th>File Name</th>
                </tr>
                </thead>
                <tbody>
                {videos.map(video =>
                    <tr key={video.id}>
                        <Link to={{
                            pathname: '/fetch-video', state: {
                                videoName: video.videoName,
                                videoUrl: video.videoUrl
                            }
                        }}>Go To Video</Link>
                        <td>{video.date}</td>
                        <td>{video.time}</td>
                        <td>{video.street}</td>
                        <td>{video.videoName}</td>
                    </tr>
                )}
                </tbody>
            </table>
        );
    }

    RenderLoading() {
        return (
            <div className={'container-fluid'}>
                <p> Loading Content...</p>
                <Loader/>
            </div>
        );
    }

    RenderHandledError() {
        return (
            <div>
                <h1>Something Went Wrong</h1>
                <h2>
                    {this.state.errorMessage}
                </h2>
            </div>
        );
    }

    render() {
        let contents = null;

        if (this.state.loading === true) {
            contents = this.RenderLoading()
        }
        if (this.state.error === true) {
            contents = this.RenderHandledError()
        }
        if (this.state.loading === false) {
            contents = this.renderTableData()
        }
        return (
            <div>
                <h1>Patterson Park Video Repository</h1>
                <p>A simple way for the community to share videos of crimes in one place</p>
                <p>Lastest Videos</p>
                {contents}
            </div>
        );
    }
}
