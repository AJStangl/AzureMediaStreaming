import React from 'react';
import {Card} from "reactstrap";
import Loader from "./Loader";

class FileUpload extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            complete: false,
            error: false,
            errorMessage: '',
            form: {
                firstName: '',
                lastName: '',
                email: '',
                phoneNumber: '',
                streetName: '',
                streetNumber: '',
                zipCode: '',
                city: '',
                state: '',
                date: '',
                time: '',
                file: null
            }
        };
    }

    handleChange(event) {
        event.preventDefault();
        let form = this.state.form;
        let name = event.target.name;
        form[name] = event.target.value;

        this.setState({form})
    }

    setFile(e) {
        this.setState({
            form: {
                file: e.target.files[0]
            }
        });
    }

    async handleSubmit(event) {
        event.preventDefault();

        const data = new FormData(event.target);
        data.set('file', this.state.file);

        this.setState({
            loading: true
        })
        await fetch('/media/video', {
            method: 'POST',
            body: data,
        }).then(async response => {
            if (response.ok) {
                await response.json().then(x => {
                    console.log("I am in the json promise" + x)
                    this.setState({complete: true, loading: false})
                })
                this.setState({complete: true, loading: false})
            } else {
                this.setState({
                    complete: false,
                    loading: false,
                    error: true,
                    errorMessage: response.statusText
                })
            }
        }).catch(err =>
            this.setState({
                loading: false,
                error: true,
                errorMessage: err
            }));
    }

    RenderLoading() {
        return (
            <div className={'container-fluid'}>
                <p> Uploading File...</p>
                <Loader/>
            </div>
        );
    }

    RenderComplete() {
        // TODO: Pass back information about the response.
        return (
            <div className="container-fluid">
                <h1>File Upload</h1>
                <p>File Uploaded</p>
            </div>
        );
    }

    SetForm() {
        // TODO: Validate all fields and prevent submission, add tooltip
        return (
            <div className="container-fluid">
                <form onSubmit={e => this.handleSubmit(e)} noValidate>
                    <div className={'info-text'}>
                        <h1>Upload File</h1>
                        {/*User and Contact Information*/}
                        <Card>
                            <div className={'form-row'}>
                                {/*Contact Information*/}
                                {/*firstName*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">First Name</label>
                                        <input type="text"
                                               name="firstName"
                                               className={'form-control user-input'}
                                               placeholder="firstName" value={this.state.form["firstName"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*lastName*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Last Name</label>
                                        <input type="text"
                                               name="lastName"
                                               className={'form-control user-input'}
                                               placeholder="lastName" value={this.state.form["lastName"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*phoneNumber*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Phone Number</label>
                                        <input type="text"
                                               name="phoneNumber"
                                               className={'form-control user-input'}
                                               placeholder="phoneNumber" value={this.state.form["phoneNumber"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*email*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Email Address</label>
                                        <input type="text"
                                               name="email"
                                               className={'form-control user-input'}
                                               placeholder="email" value={this.state.form["email"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                            </div>
                        </Card>
                        {/*Address Information*/}
                        <Card>
                            {/*Address Demographics*/}
                            <div className={'form-row'}>
                                {/*streetNumber*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Street Number</label>
                                        <input type="text"
                                               name="streetNumber"
                                               className={'form-control user-input'}
                                               placeholder="streetNumber" value={this.state.form["streetNumber"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*streetName*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Street Name</label>
                                        <input type="text"
                                               name="streetName"
                                               className={'form-control user-input'}
                                               placeholder="streetName" value={this.state.form["streetName"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*ZipCode*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Zip Code</label>
                                        <input type="text"
                                               name="zipCode"
                                               className={'form-control user-input'}
                                               placeholder="zipCode" value={this.state.form["zipCode"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*City*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">City</label>
                                        <input type="text"
                                               name="city"
                                               className={'form-control user-input'}
                                               placeholder="city" value={'Baltimore'}
                                               readOnly={'true'}
                                        />
                                    </div>
                                </div>
                                {/*State*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">State</label>
                                        <input type="text"
                                               name="state"
                                               className={'form-control user-input'}
                                               placeholder="city" value={'Md'}
                                               readOnly={'true'}
                                        />
                                    </div>
                                </div>
                            </div>
                        </Card>
                        {/*Date and Time*/}
                        <Card>
                            <div className={'form-row'}>
                                {/*Date*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Date</label>
                                        <input type="date"
                                               name="date"
                                               className={'form-control user-input'}
                                               placeholder="streetNumber" value={this.state.form["date"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                                {/*Time*/}
                                <div className="col-md-6">
                                    <div className="form-group required">
                                        <label className="font-weight-bold">Time</label>
                                        <input type="time"
                                               name="time"
                                               className={'form-control user-input'}
                                               placeholder="streetNumber" value={this.state.form["time"]}
                                               onChange={this.handleChange.bind(this)}/>
                                    </div>
                                </div>
                            </div>
                        </Card>
                        {/*File*/}
                        <Card>
                            <input type="file" onChange={e => this.setFile(e)}/>
                        </Card>
                        <button className="btn btn-primary" type="submit">Submit</button>
                    </div>
                </form>
            </div>
        )
    }

    render() {
        let contents = null;
        console.log(this.state)
        if (this.state.loading === false) {
            contents = this.SetForm()
        }
        if (this.state.loading === true) {
            contents = this.RenderLoading()
        }
        if (this.state.error === true) {
            throw new Error(this.state.errorMessage)
        }
        if (this.state.loading === false && this.state.complete === true) {
            contents = this.RenderComplete()
        }

        return (
            <div>
                {contents}
            </div>
        )
    }
}

export default FileUpload