<!-- PROJECT LOGO -->
<br />
<div align="center">
    <!-- TODO
  <a href="https://github.com/juuwel/recipe-extractor">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>
    -->

<h3 align="center">Recipe Extractor</h3>

  <p align="center">
    Extract, parse, and store recipes from websites with ease.
    <br />
    <!-- <a href="https://github.com/juuwel/recipe-extractor"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/juuwel/recipe-extractor">View Demo</a>
    &middot;
    <a href="https://github.com/juuwel/recipe-extractor/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    &middot;
    <a href="https://github.com/juuwel/recipe-extractor/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
    -->
    <a href="privacy-policy.md" target="_blank">Privacy Policy</a>
    &middot;
    <a href="terms-of-service.md" target="_blank">Terms of Service</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->

## About The Project

Recipe Extractor is a Python-based tool for extracting recipes from websites, parsing ingredients and steps, and storing them in a Notion database.

Features:

- Extract ingredients and cooking steps from recipes from various websites
- Store recipes in a Notion database

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

- [Python 3.13][Python-url]
- [BeautifulSoup][BeautifulSoup-url]
- [FastAPI][FastAPI-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->

## Getting Started

Follow these steps to set up Recipe Extractor locally.

### Prerequisites

- Python >= 3.12
- [uv][uv-url]
- [Docker][docker-url] (optional, for containerized deployment)

### Installation

1. Clone the repository

```sh
git clone https://github.com/juuwel/recipe-extractor.git
cd recipe-extractor
```

2. Install Python

```sh
uv python install
```

3. Install Python dependencies

```sh
uv sync
```

4. Run

```sh
uv run uvicorn src.main:app
```

5. (Optional) Build and run with Docker

```sh
docker compose up --build
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->

## Usage

Run the following command to launch the API server:

```sh
uvicorn main:app --reload
```

_For more details, refer to the code and documentation in the repository._

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->

## Roadmap

- [ ] Support more recipe websites
- [ ] Improve ingredient/step parsing
- [ ] Add web API interface
- [ ] Enhance Notion integration
- [ ] Add user authentication

See the [open issues][issues-url] for a full list of proposed features (and known issues).

<!-- CONTRIBUTING -->

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

<!-- LICENSE -->

## License

Distributed under the MIT License. See [LICENSE.md](LICENSE.md) for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->

## Contact

Project Link: [https://github.com/juuwel/recipe-extractor][repo-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[repo-url]: https://github.com/juuwel/recipe-extractor
[BeautifulSoup-url]: https://www.crummy.com/software/BeautifulSoup/
[FastAPI-url]: https://fastapi.tiangolo.com/
[Python-url]: https://www.python.org/
[issues-url]: https://github.com/juuwel/recipe-extractor/issues
[uv-url]: https://docs.astral.sh/uv/getting-started/installation/
[docker-url]: https://docs.docker.com/engine/install/
