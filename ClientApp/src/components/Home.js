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
        const token = authService.getAccessToken();
        await fetch('/media/LatestVideo', {
            headers: !token ? {} : {'Authorization': `Bearer ${token}`}
        }).then(response => {
            if (response.ok) {
                var json = response.json()
                this.setState({
                    videoResponse: json,
                    loading: false
                })
            }
            if (response.status === 401) {
                this.setState({
                    error: true,
                    errorMessage: "Not Authorized",
                    loading: false
                })
            }
        })
    }


    render() {
        let contents = null;

        if (this.state.loading) {
            contents = RenderLoading()
        }

        if (this.state.error) {
            contents = RenderError(this.state.errorMessage)
        }

        if (this.state.videoResponse.length === 1 && !this.state.error) {
            contents = RenderNoData();
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

function RenderNoData() {
    return (
        <div>No Data Found</div>
    )
}

function RenderError(error) {
    return (
        <div> {error} </div>
    )
}

function GetData() {
    const token = authService.getAccessToken();
    return fetch('/media/LatestVideo', {
        headers: !token ? {} : {'Authorization': `Bearer ${token}`}
    });
}

