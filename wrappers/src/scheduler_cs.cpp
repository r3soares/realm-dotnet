////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

#include <thread>
#include "util/scheduler.hpp"
#include "realm_export_decls.hpp"

using namespace realm::util;

using GetContextT = void*();
using PostOnContextT = void(void* context, void(*handler)(void* user_data), void* user_data);
using ReleaseContextT = void(void* context);

struct SynchronizationContextScheduler : public Scheduler {
public:
    SynchronizationContextScheduler(GetContextT* get, PostOnContextT* post, ReleaseContextT* release)
    : m_api({ get, post, release })
    , m_context(get())
    { }

    bool is_on_thread() const noexcept override { return m_id == std::this_thread::get_id(); }

    bool can_deliver_notifications() const noexcept override { return true; }

    void notify() override
    {
        m_api.post(m_context, &handle, this);
    }

    void set_notify_callback(std::function<void()> callback) override
    {
        m_callback = std::move(callback);
    }

    ~SynchronizationContextScheduler()
    {
        m_api.release(m_context);
    }
private:
    static void handle(void* scheduler_ptr)
    {
        auto& scheduler = *reinterpret_cast<SynchronizationContextScheduler*>(scheduler_ptr);
        scheduler.m_callback();
    }

    const std::thread::id m_id = std::this_thread::get_id();

    const struct {
        GetContextT* get;
        PostOnContextT* post;
        ReleaseContextT* release;
    } m_api;

    void* m_context;

    std::function<void()> m_callback;
};

extern "C" {

REALM_EXPORT void realm_install_eventloop_callbacks(GetContextT* get, PostOnContextT* post, ReleaseContextT* release)
{
    Scheduler::set_default_factory([=]() {
        return std::make_shared<SynchronizationContextScheduler>(get, post, release);
    });
}

}
