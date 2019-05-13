var app = new Vue({
    el: '#app',
    data: {
        uploadFile: "",
        originImage: {
            src: ""
        }
    },
    mounted: function () {
        $("#origin-img").imgAreaSelect({
            aspectRatio: "5:7",
            handles: true,
            minWidth: 250,
            minHeight:350,
            onSelectEnd: function (img, selection) {
                if (selection.width > 0) {

                }
            }
        });
    },
    methods: {
        originImgUpload: function (e) {
            e.preventDefault()
            var _this = this
            var files = e.target.files || e.dataTransfer.files;
            if (!files.length)
                return;
            var formData = new FormData();
            formData.append('file', files[0]);
            axios.post('/home/UploadOriginImg',
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                }
            ).then(function (result) {
                console.log(result)
                _this.originImage.src = result.data.oriImgSrc
                $('img#photo').imgAreaSelect({
                    handles: true,
                    onSelectEnd: someFunction
                });
            })
                .catch(function (result) {
                    console.log(result.data.messege);
                });
        },
        cropImg: function (e) {
            e.preventDefault()
        }
    }
})