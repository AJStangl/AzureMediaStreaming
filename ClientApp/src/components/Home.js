import React, {Component} from 'react';
import Loader from "./Loader";
import MaterialTable from 'material-table';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import authService from './api-authorization/AuthorizeService'

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
            loading: true,
            getData: false
        };
    }

    componentDidMount = async () => {
        const token = await authService.getAccessToken();
        return fetch('/media/LatestVideo', {
            headers: !token ? {} : {'Authorization': `Bearer ${token}`}
        })
            .then((response) => response.json())
            .then((responseData) => {
                this.setState({
                    loading: false,
                    videoResponse: responseData
                })
            })
            .catch(err => {
                this.setState({
                    error: true,
                    errorMessage: err
                })
            });
    }

    render() {
        let contents = null;
        // let dataPromise = GetLatestVideo(this.state);

        if (this.state.loading) {
            contents = RenderLoading()
        }

        if (this.state.videoResponse.length > 1) {
            contents = GetDataTable(this.state.videoResponse)
        }

        return (
            <div>
                <h1>Patterson Park Video Repository</h1>
                <p>A simple way for the community to share of crimes in one place</p>
                {contents}
            </div>
        );
    }
}

function GetDataTable(videoData) {

    let table = {
        columns: [
            {title: 'Date Recorded', field: 'date'},
            {title: 'Time Recorded', field: 'time'},
            {title: 'Address', field: 'street'},
            {title: 'File Name', field: 'videoName'}
        ],
        data: videoData,
        actions: [{
            icon: PlayArrowIcon,
            tooltip: 'Go to video',
            onClick: (event, rowData) => {
                window.location.href = '/fetch-video/' + rowData.id
            }
        }]
    }


    return (
        <MaterialTable
            title="Latest Videos"
            columns={table.columns}
            data={table.data}
            actions={table.actions}
        />
    );
}

function RenderLoading() {
    return (
        <div className={'container-fluid'}>
            <p> Loading Content...</p>
            <Loader/>
        </div>
    );
}