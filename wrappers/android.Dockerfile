FROM ubuntu:xenial

RUN apt-get update -qq \
 && apt-get install --no-install-recommends -y \
            build-essential \
            unzip \
 && apt-get clean

# Install the NDK
ADD https://dl.google.com/android/repository/android-ndk-r21-linux-x86_64.zip /tmp/ndk.zip
RUN cd /tmp \
 && unzip -qq ndk.zip \
 && mv android-ndk-r21 /opt/android-ndk \
 && rm /tmp/ndk.zip \
 && chmod -R a+rX /opt/android-ndk

ADD https://cmake.org/files/v3.17/cmake-3.17.2-Linux-x86_64.tar.gz /tmp/cmake.tar.gz
RUN cd /tmp \
 && tar -zxf cmake.tar.gz \
 && mv cmake-3.17.2-Linux-x86_64 /opt/cmake \
 && rm cmake.tar.gz

ENV PATH=/opt/cmake/bin:$PATH
ENV ANDROID_NDK=/opt/android-ndk

VOLUME /source
ENTRYPOINT ["/source/build-android.sh"]