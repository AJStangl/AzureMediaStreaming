import React from 'react';
import {Card} from "reactstrap";

class FileUpload extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            file: '',
        };
    }

    setFile(e) {
        this.setState({file: e.target.files[0]});
    }

    async handleSubmit(event) {
        event.preventDefault();
        const data = new FormData(event.target);
        data.set('file', this.state.file);

        // data.set('file', this.state.file);
        await fetch('/media/Video', {
            method: 'POST',
            body: data,
        });
    }

    render() {
        return (
            <div className="container-fluid">
                <form onSubmit={e => this.handleSubmit(e) }>
                    <h1>File Upload</h1>
                    <Card>
                        <input type="file" onChange={ e => this.setFile(e) }/>
                        <button className="btn btn-primary" type="submit">Upload</button>
                    </Card>
                </form>
            </div>

        )

    }

}

export default FileUpload